using SimpleTokenParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleTokenParser
{
    public static class FileTokenParser
    {
        public static string ModelTokenCharacter { get; set; } = "@@@";
        public static string PropertyTokenCharacter { get; set; } = "##";
        public static string TokenClassSplitCharacter { get; set; } = ".";

        public static ITokenParserModel<T> ParseTemplate<T>(string filePath) where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            var templateLines = File.ReadAllLines(filePath);
            var modelName = GetTemplateModel(templateLines);

            if (string.IsNullOrWhiteSpace(modelName))
            {
                throw new Exception("Template does not specify any model");
            }

            if (type.FullName != modelName)
            {
                throw new Exception($"Invalid model of type \"{type.FullName}\" passed. Model passed should be of type \"{modelName}\"");
            }

            var tokens = GetAllUsedTokens(templateLines);
            var tokenTest = TestTokensContainedWithinModel(properties, tokens);
            if (tokenTest.Any(i => !i.FoundInModel))
            {
                var tokensNotFoundString = String.Join("," + Environment.NewLine, tokenTest.Where(i => !i.FoundInModel).Select(i => i.Token));
                throw new Exception($"Following tokens weren't found for template \"{filePath}\"\n\n{tokensNotFoundString}");
            }

            return new TokenParserModel<T>(
                string.Join(Environment.NewLine, templateLines),
                modelName,
                tokens
            );
        }

        private static string GetTemplateModel(string[] templateLines)
        {
            foreach (var line in templateLines)
            {
                if (line.Contains(ModelTokenCharacter))
                {
                    var startIndex = line.IndexOf(ModelTokenCharacter);
                    var lastIndex = line.LastIndexOf(ModelTokenCharacter);
                    return line.Substring(
                        startIndex + ModelTokenCharacter.Length,
                        lastIndex - startIndex - ModelTokenCharacter.Length
                    );
                }
            }
            return null;
        }

        private static IEnumerable<string> GetAllUsedTokens(string[] templateLines)
        {
            var tokens = new List<string>();

            foreach (var line in templateLines)
            {
                var currentLineTokenSplits = (" " + line + " ").Split(new string[] { PropertyTokenCharacter }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < currentLineTokenSplits.Length; i++)
                {
                    if (i % 2 == 0 || tokens.Contains(currentLineTokenSplits[i])) { continue; }

                    tokens.Add(currentLineTokenSplits[i]);
                }
            }

            return tokens;
        }

        private static IEnumerable<TokenTestResult> TestTokensContainedWithinModel(
            IEnumerable<PropertyInfo> modelProperties,
            IEnumerable<string> tokens
        )
        {
            var tokenTestResults = new List<TokenTestResult>();

            foreach (var token in tokens)
            {
                var testResult = new TokenTestResult()
                {
                    Token = token,
                    FoundInModel = false,
                };

                var tokenSplits = token.Split(
                    new string[] { TokenClassSplitCharacter },
                    StringSplitOptions.RemoveEmptyEntries
                );
                var property = modelProperties.FirstOrDefault(i => i.Name == tokenSplits[0]);
                if (property == null)
                {
                    tokenTestResults.Add(testResult);
                    continue;
                }

                if (token.Contains('.'))
                {
                    var result = TestTokensContainedWithinModel(
                            property.PropertyType.GetProperties(),
                            new string[]
                            {
                                string.Join(
                                    TokenClassSplitCharacter,
                                    tokenSplits.ToList().Skip(1)
                                )
                            }
                        ).FirstOrDefault();

                    if (result.FoundInModel)
                    {
                        testResult.FoundInModel = true;
                    }
                    tokenTestResults.Add(testResult);
                    continue;
                }
                testResult.FoundInModel = true;

                tokenTestResults.Add(testResult);
            }

            return tokenTestResults;
        }
    }
}
