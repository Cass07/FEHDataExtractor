using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FEHDataExtractor
{
    public partial class Form1 : Form
    {
        private ExtractionBase[] a;
        public static int offset = 0x20;
        private String Path;
        private String[] Pathes;
        private String MessagePath;

        public ExtractionBase[] A { get => a; set => a = value; }

        public Form1(params ExtractionBase[] a)
        {
            this.A = a;
            InitializeComponent();
            for (int i = 0; i < A.Length; i++)
            {
                this.comboBox1.Items.Add(A[i].Name);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Path = openFileDialog1.FileName;
                Pathes = openFileDialog1.FileNames;

                FileListBox.Items.Clear();

                foreach (String file in Pathes)
                {
                    FileListBox.Items.Add(file);
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Path = "";
            openFileDialog1.FileName = "";
            openFileDialog1.Reset();
            FileListBox.Items.Clear();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("https://github.com/Cass07/FEHDataExtractor", "About");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Path != null && Path != "" && !(comboBox1.SelectedItem == null || comboBox1.SelectedItem.ToString().Equals("")))
            {
                ExtractionBase tmp = null;
                for (int i = 0; i < A.Length; i++)
                    if (comboBox1.SelectedItem.ToString().Equals(A[i].Name))
                        tmp = A[i];
                foreach (String file in Pathes)
                {
                    string ext = System.IO.Path.GetExtension(file).ToLower();
                    byte[] data = Decompression.Open(file);
                    String output = "";

                    if (data != null && tmp != null && !(tmp.Name.Equals("") || tmp.Name.Equals("Decompress")))
                    {
                        HSDARC a = new HSDARC(0, data);
                        while (a.Ptr_list_length - a.NegateIndex > a.Index)
                        {
                            tmp.InsertIn(a, offset, data);
                            output += tmp.ToString();
                        }
                    }

                    String PathManip = file.Remove(file.Length - 3, 3);
                    if (ext.Equals(".lz"))
                        PathManip = file.Remove(file.Length - 6, 6);
                    PathManip += tmp.Name.Equals("Decompress") ? "bin" : "txt";
                    if (file.Equals(PathManip))
                        PathManip += tmp.Name.Equals("Decompress") ? ".bin" : ".txt";
                    if (tmp.Name.Equals("Decompress") && data != null)
                        File.WriteAllBytes(PathManip, data);
                    else
                        File.WriteAllText(PathManip, output);
                }
                MessageBox.Show(Pathes.Length > 1 ? "Files processed!" : "File processed!", "Success");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void messagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                MessagePath = folderBrowserDialog1.SelectedPath;
                LoadMessages.openFolder(MessagePath);
                MessageBox.Show("Loaded messages!", "Success");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Path != null && Path != "" && !(comboBox1.SelectedItem == null || comboBox1.SelectedItem.ToString().Equals("")))
            {
                string selectedExtractorName = comboBox1.SelectedItem.ToString();
                ExtractionBase originalTmp = null;

                for (int i = 0; i < A.Length; i++)
                    if (selectedExtractorName.Equals(A[i].Name))
                        originalTmp = A[i];

                if (originalTmp == null)
                {
                    MessageBox.Show("Invalid extraction type selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 로그 표시 설정 확인
                bool showLog = showProsessLogToolStripMenuItem.Checked;

                // 로그 폼 객체
                LogForm logForm = null;

                // 로그 표시 설정이 켜져 있을 경우에만 로그 폼 생성 및 표시
                if (showLog)
                {
                    logForm = new LogForm();
                    logForm.Show();
                    logForm.AddLog("Starting JSON processing...");
                }

                // 모든 파일 리스트 수집
                List<string> allFilesToProcess = new List<string>();

                foreach (String path in Pathes)
                {
                    if (Directory.Exists(path))
                    {
                        // 폴더인 경우 재귀적으로 모든 .lz 파일 검색
                        try
                        {
                            // 재귀 검색 함수 호출
                            FindFilesRecursively(path, allFilesToProcess, "*.lz");

                            if (showLog)
                            {
                                logForm.AddLog($"Searched directory: {path}");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (showLog)
                            {
                                logForm.AddLog($"Error searching directory {path}: {ex.Message}", ex);
                            }
                        }
                    }
                    else if (File.Exists(path))
                    {
                        // 파일인 경우 직접 추가
                        string ext = System.IO.Path.GetExtension(path).ToLower();
                        if (ext.Equals(".lz"))
                        {
                            allFilesToProcess.Add(path);
                        }
                    }
                }

                // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                if (showLog)
                {
                    logForm.AddLog($"Found {allFilesToProcess.Count} files to process");
                }

                // 뷰티파이 설정 저장
                bool beautify = checkBox1.Checked;

                // 완료된 파일 카운터
                int completedFiles = 0;
                int errorFiles = 0;
                object lockObj = new object();

                // 팩토리가 초기화되었는지 확인
                if (!ExtractionBaseFactory.IsInitialized)
                {
                    ExtractionBaseFactory.Initialize(A);

                    // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                    if (showLog)
                    {
                        logForm.AddLog("Factory initialized with available extractors");
                    }
                }

                // 멀티스레드 처리 시작
                Task.Run(() =>
                {
                    // 처리할 파일 수에 따라 적절한 스레드 수 결정 (최대 환경 프로세서 수)
                    int threadCount = Math.Min(Environment.ProcessorCount, allFilesToProcess.Count);
                    threadCount = Math.Max(1, threadCount); // 최소한 1개 스레드 보장

                    // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                    if (showLog)
                    {
                        logForm.AddLog($"Starting processing with {threadCount} threads");
                    }

                    // 병렬 처리 수행
                    Parallel.ForEach(allFilesToProcess, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, file =>
                    {
                        try
                        {
                            // 팩토리를 사용하여 각 작업마다 새 인스턴스 생성
                            ExtractionBase threadLocalTmp = ExtractionBaseFactory.Create(selectedExtractorName);

                            if (threadLocalTmp == null)
                            {
                                // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                                if (showLog)
                                {
                                    logForm.AddLog($"Error: Failed to create extractor for {selectedExtractorName}",
                                        new Exception("Factory failed to create extractor instance"));
                                }
                                lock (lockObj) { errorFiles++; }
                                return;
                            }

                            string ext = System.IO.Path.GetExtension(file).ToLower();
                            byte[] data = null;

                            try
                            {
                                data = Decompression.Open(file);
                            }
                            catch (Exception ex)
                            {
                                // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                                if (showLog)
                                {
                                    logForm.AddLog($"Error decompressing {file}: {ex.Message}", ex);
                                }
                                lock (lockObj) { errorFiles++; }
                                return;
                            }

                            String output = "";

                            if (data != null && threadLocalTmp != null && !(threadLocalTmp.Name.Equals("") || threadLocalTmp.Name.Equals("Decompress")))
                            {
                                try
                                {
                                    HSDARC a = new HSDARC(0, data);
                                    while (a.Ptr_list_length - a.NegateIndex > a.Index)
                                    {
                                        if (!threadLocalTmp.Name.Equals("Messages"))
                                        {
                                            threadLocalTmp.InsertIn(a, 0, data);
                                        }
                                        else
                                        {
                                            threadLocalTmp.InsertIn(a, offset, data);
                                        }

                                        if (beautify && threadLocalTmp is Messages)
                                        {
                                            output = ((Messages)threadLocalTmp).ToJsonBeautfy();
                                        }
                                        else
                                        {
                                            output += threadLocalTmp.ToString_json();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                                    if (showLog)
                                    {
                                        logForm.AddLog($"Error processing {file}: {ex.Message}", ex);
                                    }
                                    lock (lockObj) { errorFiles++; }
                                    return;
                                }
                            }

                            try
                            {
                                if (!(threadLocalTmp is Messages) && !string.IsNullOrEmpty(output))
                                {
                                    output = "[" + output.Substring(0, output.Length - 1) + "]";
                                }

                                if (beautify && !(threadLocalTmp is Messages) && !string.IsNullOrEmpty(output))
                                {
                                    output = JValue.Parse(output).ToString(Newtonsoft.Json.Formatting.Indented);
                                }

                                String PathManip = file.Remove(file.Length - 3, 3);
                                if (ext.Equals(".lz"))
                                    PathManip = file.Remove(file.Length - 6, 6);
                                PathManip += threadLocalTmp.Name.Equals("Decompress") ? "bin" : "json";
                                if (file.Equals(PathManip))
                                    PathManip += threadLocalTmp.Name.Equals("Decompress") ? ".bin" : ".json";

                                if (threadLocalTmp.Name.Equals("Decompress") && data != null)
                                    File.WriteAllBytes(PathManip, data);
                                else if (threadLocalTmp.Name.Equals("Messages") && data != null)
                                    File.WriteAllBytes(PathManip, Encoding.UTF8.GetBytes(output));
                                else if (!string.IsNullOrEmpty(output))
                                    File.WriteAllText(PathManip, output);
                                else
                                    throw new Exception("No output generated");
                            }
                            catch (Exception ex)
                            {
                                // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                                if (showLog)
                                {
                                    logForm.AddLog($"Error writing output for {file}: {ex.Message}", ex);
                                }
                                lock (lockObj) { errorFiles++; }
                                return;
                            }

                            // 로그 업데이트 - 로그 표시 설정이 켜져 있을 경우에만
                            if (showLog)
                            {
                                string fileName = System.IO.Path.GetFileName(file);
                                logForm.AddLog($"Processed: {fileName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                            if (showLog)
                            {
                                logForm.AddLog($"Unexpected error processing {file}: {ex.Message}", ex);
                            }
                            lock (lockObj) { errorFiles++; }
                        }
                    });

                    // 모든 파일 처리 완료
                    this.Invoke(new Action(() =>
                    {
                        completedFiles = allFilesToProcess.Count - errorFiles;
                        // 로그 표시 설정이 켜져 있을 경우에만 로그 추가
                        if (showLog)
                        {
                            logForm.AddLog($"All files processed. Success: {completedFiles}, Errors: {errorFiles}");
                        }

                        if (errorFiles > 0)
                            MessageBox.Show($"처리 완료. 성공: {completedFiles}, 오류: {errorFiles}", "처리 완료",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MessageBox.Show(Pathes.Length > 1 ? "JSON Files processed!" : "JSON File processed!", "Success");
                    }));
                });
            }
        }

        /// <summary>
        /// 지정된 폴더와 모든 하위 폴더에서 .lz 파일을 재귀적으로 검색합니다.
        /// </summary>
        /// <param name="folderPath">검색할 폴더 경로</param>
        /// <param name="fileList">발견된 파일이 추가될 리스트</param>
        private void FindFilesRecursively(string folderPath, List<string> fileList, String extension)
        {
            try
            {
                // 현재 폴더의 모든 .lz 파일 검색
                foreach (string file in Directory.GetFiles(folderPath, extension))
                {
                    fileList.Add(file);
                }

                // 모든 하위 폴더에 대해 재귀 호출
                foreach (string subDir in Directory.GetDirectories(folderPath))
                {
                    FindFilesRecursively(subDir, fileList, extension);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // 접근 권한이 없는 폴더는 스킵
                System.Diagnostics.Debug.WriteLine($"Access denied to {folderPath}: {ex.Message}");
            }
            catch (PathTooLongException ex)
            {
                // 경로가 너무 긴 경우 스킵
                System.Diagnostics.Debug.WriteLine($"Path too long in {folderPath}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // 기타 예외 처리
                System.Diagnostics.Debug.WriteLine($"Error searching {folderPath}: {ex.Message}");
            }
        }

        private void File_DragDrop(object sender, DragEventArgs e)
        {
            string[] directoryName = (string[])e.Data.GetData(DataFormats.FileDrop);

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            FileListBox.Items.Clear();
            Pathes = files;
            Path = Pathes[0];

            foreach (String file in Pathes)
            {
                FileListBox.Items.Add(file);
            }
        }

        private void File_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
