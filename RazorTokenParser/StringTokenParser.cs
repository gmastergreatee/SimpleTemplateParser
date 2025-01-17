using SimpleTokenParser.Exceptions;
using SimpleTokenParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleTokenParser
{
    public static class StringTokenParser
    {
        /// <summary>
        /// Generate token based string-parser object. No support for array-types within model.
        /// </summary>
        /// <typeparam name="T">The binding model for the template file-contents.</typeparam>
        /// <param name="fileContents">The contents of the template</param>
        /// <returns></returns>
        /// <exception cref="ModelNotFoundException">The absense of model in template will throw and exception</exception>
        /// <exception cref="TokenNotFoundException">All tokens used in the template file must be properties found in the model type</exception>
        public static ITokenParserModel<T> ParseTemplate<T>(string fileContents, bool ignoreUnavailableTokens = false) where T : class
        {
            var templateLines = fileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var modelName = GetTemplateModel(templateLines);

            if (string.IsNullOrWhiteSpace(modelName))
            {
                throw new ModelNotFoundException("Template does not specify any model");
            }

            var type = typeof(T);
            if (!ignoreUnavailableTokens && type.FullName != modelName)
            {
                throw new Exception($"Invalid model of type \"{type.FullName}\" passed. Model passed should be of type \"{modelName}\"");
            }

            var properties = type.GetProperties();
            var tokens = GetAllUsedTokens(templateLines);

            if (!ignoreUnavailableTokens)
            {
                var tokenTest = TestTokensContainedWithinModel(properties, tokens);
                if (tokenTest.Any(i => !i.FoundInModel))
                {
                    var tokensNotFoundString = String.Join("," + Environment.NewLine, tokenTest.Where(i => !i.FoundInModel).Select(i => i.Token));
                    throw new TokenNotFoundException($"Following tokens weren't found for template\n\n{tokensNotFoundString}");
                }
            }

            return new TokenParserModel<T>(
                string.Join(Environment.NewLine, templateLines),
                modelName,
                tokens,
                ignoreUnavailableTokens
            );
        }

        private static string GetTemplateModel(IEnumerable<string> templateLines)
        {
            foreach (var line in templateLines)
            {
                if (line.Contains(TokenParserConstants.ModelTokenCharacter))
                {
                    var startIndex = line.IndexOf(TokenParserConstants.ModelTokenCharacter);
                    var lastIndex = line.LastIndexOf(TokenParserConstants.ModelTokenCharacter);
                    return line.Substring(
                        startIndex + TokenParserConstants.ModelTokenCharacter.Length,
                        lastIndex - startIndex - TokenParserConstants.ModelTokenCharacter.Length
                    );
                }
            }
            return null;
        }

        private static IEnumerable<string> GetAllUsedTokens(IEnumerable<string> templateLines)
        {
            var tokens = new List<string>();

            foreach (var line in templateLines)
            {
                var currentLineTokenSplits = (" " + line + " ").Split(new string[] { TokenParserConstants.PropertyTokenCharacter }, StringSplitOptions.None);
                for (var i = 0; i < currentLineTokenSplits.Length; i++)
                {
                    var currentToken = currentLineTokenSplits[i];
                    if (
                        i % 2 == 0 ||
                        tokens.Contains(currentToken) ||
                        TokenParserConstants.IgnoreTokens.Contains(currentToken)
                    ) { continue; }

                    tokens.Add(currentToken);
                }
            }

            return tokens;
        }

        private static IEnumerable<TokenTestResultModel> TestTokensContainedWithinModel(
            IEnumerable<PropertyInfo> modelProperties,
            IEnumerable<string> tokens
        )
        {
            var tokenTestResults = new List<TokenTestResultModel>();

            foreach (var token in tokens)
            {
                if (TokenParserConstants.IgnoreTokens.Contains(token))
                {
                    continue;
                }

                var testResult = new TokenTestResultModel()
                {
                    Token = token,
                    FoundInModel = false,
                };

                var tokenSplits = token.Split(
                    new string[] { TokenParserConstants.TokenClassSplitCharacter },
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
                                    TokenParserConstants.TokenClassSplitCharacter,
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
