
using System;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Utilities;

namespace Mercraft.Core.MapCss.Domain
{
    public abstract class Selector
    {
        // TODO process pseudo selectors in different way
        // IsClosed used only by way
        public bool IsClosed { get; set; }

        public string Tag { get; set; }
        public string Value { get; set; }
        public string Operation { get; set; }

        public abstract bool IsApplicable(Model model);

        protected bool CheckModel<T>(Model model) where T: Model
        {
            if (!(model is T))
                return false;

            return MatchTags(model);
        }

        protected bool MatchTags(Model model)
        {
            switch (Operation)
            {
                case MapCssStrings.OperationEquals:
                    return model.Tags.ContainsKeyValue(Tag, Value);
                case MapCssStrings.OperationExist:
                    return model.Tags.ContainsKey(Tag);
                default:
                    throw new MapCssFormatException(null, String.Format("Unsupported selector operation: {0}", model));
            }
        }
    }

    #region Concret trivial implementations

    public class NodeSelector : Selector
    {
        public override bool IsApplicable(Model model)
        {
            return CheckModel<Node>(model);
        }
    }

    public class AreaSelector : Selector
    {
        public override bool IsApplicable(Model model)
        {
            return CheckModel<Area>(model);
        }
    }

    public class WaySelector : Selector
    {
        public override bool IsApplicable(Model model)
        {
            return IsClosed ? model.IsClosed: CheckModel<Way>(model);
        }
    }

    public class CanvasSelector : Selector
    {
        public override bool IsApplicable(Model model)
        {
            return model is Canvas;
        }
    }

    #endregion
}
