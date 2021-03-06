﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ActionStreetMap.Osm.Entities;
using ActionStreetMap.Osm.Format.Xml.v0_6;
using ActionStreetMap.Osm.Formats.Xml;
using Ionic.Zlib;

namespace ActionStreetMap.Osm.Data
{
    /// <summary>
    ///     A stream reader that reads from OSM Xml.
    /// </summary>
    public class XmlElementSource : StatefulElementSource, IEnumerator<Element>, IEnumerable<Element>
    {
        private XmlReader _reader;

        private XmlSerializer _serNode;

        private XmlSerializer _serWay;

        private XmlSerializer _serRelation;

        private Element _next;

        private Stream _stream;

        private readonly bool _gzip;

        private bool _disposeStream = false;

        /// <summary>
        ///     Creates a new OSM Xml processor source.
        /// </summary>
        public XmlElementSource(Stream stream) :
            this(stream, false)
        {
        }

        /// <summary>
        ///     Creates a new OSM XML processor source.
        /// </summary>
        public XmlElementSource(Stream stream, bool gzip)
        {
            _stream = stream;
            _gzip = gzip;
        }

        /// <summary>
        ///     Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _next = null;
            _serNode = new XmlSerializer(typeof (node));
            _serWay = new XmlSerializer(typeof (way));
            _serRelation = new XmlSerializer(typeof (relation));

            Reset();
            Initialize(this);
        }

        /// <summary>
        ///     Resets this source.
        /// </summary>
        public override void Reset()
        {
            // create the xml reader settings.
            var settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.CheckCharacters = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            //settings.IgnoreWhitespace = true;

            // seek to the beginning of the stream.
            if (_stream.CanSeek)
            {
                // if a non-seekable stream is given resetting is disabled.
                _stream.Seek(0, SeekOrigin.Begin);
            }

            // decompress if needed.
            if (_gzip)
            {
                _stream = new GZipStream(_stream, CompressionMode.Decompress);
            }

            TextReader textReader = new StreamReader(_stream, Encoding.UTF8);
            _reader = XmlReader.Create(textReader, settings);
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        ///     Returns true if this source can be reset.
        /// </summary>
        public bool CanReset
        {
            get { return _stream.CanSeek; }
        }

        /// <summary>
        ///     Moves this source to the next object.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element &&
                    (_reader.Name == "node" || _reader.Name == "way" || _reader.Name == "relation"))
                {
                    // create a stream for only this element.
                    string name = _reader.Name;
                    string nextElement = _reader.ReadOuterXml();
                    XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(nextElement)));
                    object osmObj;

                    // select type of element.
                    switch (name)
                    {
                        case "node":
                            osmObj = _serNode.Deserialize(reader);
                            if (osmObj is node)
                            {
                                _next =
                                    XmlSimpleConverter.ConvertToSimple(osmObj as node);
                                return true;
                            }
                            break;
                        case "way":
                            osmObj = _serWay.Deserialize(reader);
                            if (osmObj is way)
                            {
                                _next =
                                    XmlSimpleConverter.ConvertToSimple(osmObj as way);
                                return true;
                            }
                            break;
                        case "relation":
                            osmObj = _serRelation.Deserialize(reader);
                            if (osmObj is relation)
                            {
                                _next =
                                    XmlSimpleConverter.ConvertToSimple(
                                        osmObj as relation);
                                return true;
                            }
                            break;
                    }
                }
            }
            _next = null;
            return false;
        }

        /// <summary>
        ///     Returns the current object.
        /// </summary>
        public Element Current
        {
            get { return _next; }
        }

        /// <summary>
        ///     Disposes all resources associated with this stream.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _disposeStream)
            {
                _stream.Dispose();
            }
        }

        /// <inheritdoc />
        public IEnumerator<Element> GetEnumerator()
        {
            return this;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}