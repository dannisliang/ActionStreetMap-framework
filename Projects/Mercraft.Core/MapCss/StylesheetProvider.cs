using System.IO;
using Antlr.Runtime;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.MapCss.Visitors;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Core.MapCss
{
    public interface IStylesheetProvider
    {
        Stylesheet Get();
    }

    /// <summary>
    ///     Default implementation of IStylesheetProvider
    /// </summary>
    public class StylesheetProvider : IStylesheetProvider, IConfigurable
    {
        private const string PathKey = "path";

        private string _path;
        private Stylesheet _stylesheet;

        #region Constructors

        [Dependency]
        public StylesheetProvider()
        {
        }

        public StylesheetProvider(string path)
        {
            _path = path;
        }

        public StylesheetProvider(Stream stream)
        {
            _stylesheet = Create(stream);
        }

        #endregion

        public Stylesheet Get()
        {
            if (_stylesheet == null)
                _stylesheet = Create();

            return _stylesheet;
        }

        public void Configure(IConfigSection configSection)
        {
            _path = configSection.GetString(PathKey);
        }

        private Stylesheet Create()
        {
            using (Stream inputStream = File.Open(_path, FileMode.Open))
            {
                return Create(inputStream);
            }
        }

        private static Stylesheet Create(Stream stream)
        {
            var input = new ANTLRInputStream(stream);
            var lexer = new MapCssLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new MapCssParser(tokens);

            var styleSheet = parser.stylesheet();
            var tree = styleSheet.Tree as Antlr.Runtime.Tree.CommonTree;

            var visitor = new MapCssVisitor();
            return visitor.Visit(tree);
        }
    }
}