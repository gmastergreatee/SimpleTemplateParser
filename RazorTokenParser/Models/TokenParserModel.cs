using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SimpleTokenParser.Models
{
    internal class TokenParserModel<T> : ITokenParserModel<T> where T : class
    {
        public string TemplateContent { get; }
        public string ModelName { get; }
        public IEnumerable<string> Tokens { get; }

        public TokenParserModel(string content, string modelName, IEnumerable<string> tokens)
        {
            this.TemplateContent = content;
            ModelName = modelName;
            this.Tokens = tokens;
        }

        public string ApplyModel(T model)
        {
            var finalContent = new StringBuilder(TemplateContent);
            finalContent = finalContent
                .Replace(
                    $"{TokenParserConstants.ModelTokenCharacter}{ModelName}{TokenParserConstants.ModelTokenCharacter}",
                    ""
                );

            foreach (var token in this.Tokens)
            {
                var tokenValue = ResolveTokenValue(token, model);
                finalContent = finalContent.Replace(
                    $"{TokenParserConstants.PropertyTokenCharacter}{token}{TokenParserConstants.PropertyTokenCharacter}",
                    tokenValue
                );
            }
            return finalContent.ToString().Trim();
        }

        private string ResolveTokenValue(string token, dynamic model)
        {
            var properties = model.GetType().GetProperties() as PropertyInfo[];
            var tokenSplits = token.Split(
                new string[] { TokenParserConstants.TokenClassSplitCharacter },
                StringSplitOptions.RemoveEmptyEntries
            );
            var property = properties.FirstOrDefault(i => i.Name == tokenSplits[0]);
            if (property == null)
            {
                throw new Exception($"Cannot find property in model with name \"{tokenSplits[0]}\"");
            }

            if (tokenSplits.Length > 1)
            {
                return ResolveTokenValue(
                    string.Join(
                        TokenParserConstants.TokenClassSplitCharacter,
                        tokenSplits.Skip(1)
                    ),
                    property.GetValue(model)
                );
            }
            return property.GetValue(model).ToString();
        }
    }
}
