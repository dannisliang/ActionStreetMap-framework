
using System.IO;
using Antlr.Runtime;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.MapCss.Visitors;

namespace Mercraft.Core.MapCss
{
    public class StylesheetProvider
    {
        private readonly string _path;
        public StylesheetProvider(string path)
        {
            _path = path;
        }
        public Stylesheet Get()
        {
            using (Stream inputStream = File.Open(_path, FileMode.Open))
            {
                var input = new ANTLRInputStream(inputStream);
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
}
