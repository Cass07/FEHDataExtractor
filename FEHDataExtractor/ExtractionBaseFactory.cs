using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FEHDataExtractor
{
    /// <summary>
    /// ExtractionBase ��ü�� �����ϴ� ���丮 Ŭ����
    /// </summary>
    public static class ExtractionBaseFactory
    {
        private static Dictionary<string, Func<ExtractionBase>> extractorFactories = new Dictionary<string, Func<ExtractionBase>>();
        private static bool isInitialized = false;

        /// <summary>
        /// ���丮�� �ʱ�ȭ�ϰ� ����� �ν��Ͻ��� ����մϴ�.
        /// </summary>
        /// <param name="extractors">����� ����� �ν��Ͻ���</param>
        public static void Initialize(ExtractionBase[] extractors)
        {
            if (isInitialized)
                return;

            foreach (var extractor in extractors)
            {
                if (extractor == null || string.IsNullOrEmpty(extractor.Name))
                    continue;

                Type extractorType = extractor.GetType();

                RegisterExtractor(extractor.Name, extractorType);
            }

            isInitialized = true;
        }

        /// <summary>
        /// �Ϲ� ����� Ÿ���� ���丮�� ����մϴ�.
        /// </summary>
        private static void RegisterExtractor(string name, Type type)
        {
            try
            {
                extractorFactories[name] = () =>
                {
                    try
                    {
                        // �Ű����� ���� �����ڸ� ����Ͽ� �ν��Ͻ� ����
                        return (ExtractionBase)Activator.CreateInstance(type);
                    }
                    catch (MissingMethodException)
                    {
                        // �Ű����� ���� �����ڰ� ���� ��� �߰ߵ� ù ��° �����ڷ� �õ�
                        ConstructorInfo[] constructors = type.GetConstructors();
                        if (constructors.Length > 0)
                        {
                            ParameterInfo[] parameters = constructors[0].GetParameters();
                            object[] paramValues = new object[parameters.Length];

                            // �Ű����� �⺻�� ����
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].ParameterType.IsValueType)
                                    paramValues[i] = Activator.CreateInstance(parameters[i].ParameterType);
                                else
                                    paramValues[i] = null;
                            }

                            return (ExtractionBase)constructors[0].Invoke(paramValues);
                        }
                        throw;
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to register extractor {name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Ư�� �̸��� ExtractionBase �ν��Ͻ��� �����մϴ�.
        /// </summary>
        /// <param name="name">������ ������� �̸�</param>
        /// <returns>�� ExtractionBase �ν��Ͻ� �Ǵ� ���� �� null</returns>
        public static ExtractionBase Create(string name)
        {
            if (!isInitialized)
                throw new InvalidOperationException("Factory not initialized. Call Initialize method first.");

            if (extractorFactories.TryGetValue(name, out Func<ExtractionBase> factory))
            {
                try
                {
                    return factory();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating instance of {name}: {ex.Message}");
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// ���丮�� �ʱ�ȭ�Ǿ����� Ȯ���մϴ�.
        /// </summary>
        public static bool IsInitialized => isInitialized;
    }
}
