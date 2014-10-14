using System.IO;
using Antlr.Runtime;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.MapCss.Visitors;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.IO;

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
        private readonly IFileSystemService _fileSystemService;
        private const string PathKey = "mapcss";
        private const string SandboxKey = "sandbox";

        private string _path;
        private bool _isSandbox;
        
        private Stylesheet _stylesheet;

        #region Constructors

        [Dependency]
        public StylesheetProvider(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public StylesheetProvider(string path, IFileSystemService fileSystemService)
            : this(fileSystemService)
        {
            _path = path;
        }

        public StylesheetProvider(Stream stream, bool canUseExprTree)
        {
            _stylesheet = Create(stream, canUseExprTree);
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
            _isSandbox = configSection.GetBool(SandboxKey);
            _stylesheet = null;
        }

        private Stylesheet Create()
        {
            using (Stream inputStream = _fileSystemService.ReadStream(_path))
            {
                return Create(inputStream, _isSandbox);
            }
        }

        private static Stylesheet Create(Stream stream, bool isSandbox)
        {
            var input = new ANTLRInputStream(stream);
            var lexer = new MapCssLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new MapCssParser(tokens);

            var styleSheet = parser.stylesheet();
            var tree = styleSheet.Tree as Antlr.Runtime.Tree.CommonTree;
            // NOTE we cannot use expression trees in sandbox (e.g. web player)
            bool canUseExprTree = !isSandbox;
            var visitor = new MapCssVisitor(canUseExprTree);
            return visitor.Visit(tree);
        }
    }
}