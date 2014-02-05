using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Streams
{
    /// <summary>
    /// Any target of osm data (NodeIds, Ways and Relations).
    /// </summary>
    public abstract class OsmStreamTarget
    {
        /// <summary>
        /// Holds the source for this target.
        /// </summary>
        private OsmStreamSource _source;

        /// <summary>
        /// Creates a new target.
        /// </summary>
        protected OsmStreamTarget(OsmStreamSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Initializes the target.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Adds a node to the target.
        /// </summary>
        /// <param name="simpleNode"></param>
        public abstract void AddNode(Node simpleNode);

        /// <summary>
        /// Adds a way to the target.
        /// </summary>
        /// <param name="simpleWay"></param>
        public abstract void AddWay(Way simpleWay);

        /// <summary>
        /// Adds a relation to the target.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public abstract void AddRelation(Relation simpleRelation);


        /// <summary>
        /// Returns the registered reader.
        /// </summary>
        protected OsmStreamSource Source
        {
            get
            {
                return _source;
            }
        }

        /// <summary>
        /// Pulls the changes from the source to this target.
        /// </summary>
        public void Pull()
        {
            _source.Initialize();
            this.Initialize();
            if (this.OnBeforePull())
            {
                this.DoPull();
                this.OnAfterPull();
            }
            this.Flush();
            this.Close();
        }

        /// <summary>
        /// Pulls the next object and returns true if there was one.
        /// </summary>
        /// <returns></returns>
        public bool PullNext()
        {
            if (_source.MoveNext())
            {
                object sourceObject = _source.Current();
                if (sourceObject is Node)
                {
                    this.AddNode(sourceObject as Node);
                }
                else if (sourceObject is Way)
                {
                    this.AddWay(sourceObject as Way);
                }
                else if (sourceObject is Relation)
                {
                    this.AddRelation(sourceObject as Relation);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Does the pull operation until source is exhausted.
        /// </summary>
        protected void DoPull()
        {
            while (_source.MoveNext())
            {
                object sourceObject = _source.Current();
                if (sourceObject is Node)
                {
                    this.AddNode(sourceObject as Node);
                }
                else if (sourceObject is Way)
                {
                    this.AddWay(sourceObject as Way);
                }
                else if (sourceObject is Relation)
                {
                    this.AddRelation(sourceObject as Relation);
                }
            }
        }

        /// <summary>
        /// Called right before pull and right after initialization.
        /// </summary>
        public virtual bool OnBeforePull()
        {
            return true;
        }

        /// <summary>
        /// Called right after pull and right before flush.
        /// </summary>
        public virtual void OnAfterPull()
        {

        }

        /// <summary>
        /// Closes the current target.
        /// </summary>
        public virtual void Close()
        {

        }

        /// <summary>
        /// Flushes the current target.
        /// </summary>
        public virtual void Flush()
        {

        }
    }
}