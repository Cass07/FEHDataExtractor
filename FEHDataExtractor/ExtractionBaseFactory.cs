using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FEHDataExtractor
{
    /// <summary>
    /// ExtractionBase 객체를 생성하는 팩토리 클래스
    /// </summary>
    public static class ExtractionBaseFactory
    {
        private static Dictionary<string, Func<ExtractionBase>> extractorFactories = new Dictionary<string, Func<ExtractionBase>>();
        private static bool isInitialized = false;

        /// <summary>
        /// 팩토리를 초기화하고 추출기 인스턴스를 등록합니다.
        /// </summary>
        /// <param name="extractors">등록할 추출기 인스턴스들</param>
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
        /// 일반 추출기 타입을 팩토리에 등록합니다.
        /// </summary>
        private static void RegisterExtractor(string name, Type type)
        {
            try
            {
                extractorFactories[name] = () =>
                {
                    try
                    {
                        // 매개변수 없는 생성자를 사용하여 인스턴스 생성
                        return (ExtractionBase)Activator.CreateInstance(type);
                    }
                    catch (MissingMethodException)
                    {
                        // 매개변수 없는 생성자가 없는 경우 발견된 첫 번째 생성자로 시도
                        ConstructorInfo[] constructors = type.GetConstructors();
                        if (constructors.Length > 0)
                        {
                            ParameterInfo[] parameters = constructors[0].GetParameters();
                            object[] paramValues = new object[parameters.Length];

                            // 매개변수 기본값 설정
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
        /// 특정 이름의 ExtractionBase 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="name">생성할 추출기의 이름</param>
        /// <returns>새 ExtractionBase 인스턴스 또는 실패 시 null</returns>
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
        /// 팩토리가 초기화되었는지 확인합니다.
        /// </summary>
        public static bool IsInitialized => isInitialized;
    }
}
