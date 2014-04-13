using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Mercraft.Models.Buildings.Entities;

namespace Mercraft.Models.Buildings
{
    public class ConstraitProvider
    {
        private static Dictionary<string, GenerateConstraints> _constraintsMap =
            new Dictionary<string, GenerateConstraints>();

        public static GenerateConstraints Get(string dataFilePath)
        {

            if (_constraintsMap.ContainsKey(dataFilePath))
                return _constraintsMap[dataFilePath];

            var constraints = new GenerateConstraints();

            constraints.Init();

            if (!File.Exists(dataFilePath))
                throw new ArgumentException("Unable to find constrait file", dataFilePath);

            XmlDocument xml = new XmlDocument();
            StreamReader sr = new StreamReader(dataFilePath);
            xml.LoadXml(sr.ReadToEnd());
            sr.Close();
            var xmlData = xml.SelectNodes("data");
            if (xmlData.Count > 0)
            {
                if (xmlData[0]["datatype"].FirstChild.Value != "ProGen")
                    xmlData = null;
            }


            if (xmlData == null)
                throw new ArgumentException("Unable to read constraints file");

            XmlNode node = (xmlData[0]["constraint"]);

            constraints.useSeed = node["useSeed"].FirstChild.Value == "True";
            constraints.seed = int.Parse(node["seed"].FirstChild.Value);
            constraints.minimumFloorHeight = float.Parse(node["minimumFloorHeight"].FirstChild.Value);
            constraints.maximumFloorHeight = float.Parse(node["maximumFloorHeight"].FirstChild.Value);
            constraints.constrainHeight = node["constrainHeight"].FirstChild.Value == "True";
            constraints.minimumHeight = float.Parse(node["minimumHeight"].FirstChild.Value);
            constraints.maximumHeight = float.Parse(node["maximumHeight"].FirstChild.Value);
            constraints.constrainFloorNumber = node["constrainFloorNumber"].FirstChild.Value == "True";
            constraints.floorNumber = int.Parse(node["floorNumber"].FirstChild.Value);
            constraints.constrainPlanByArea = node["constrainPlanByArea"].FirstChild.Value == "True";
            constraints.area.x = float.Parse(node["areax"].FirstChild.Value);
            constraints.area.y = float.Parse(node["areay"].FirstChild.Value);
            constraints.area.width = float.Parse(node["areawidth"].FirstChild.Value);
            constraints.area.height = float.Parse(node["areaheight"].FirstChild.Value);
            constraints.constrainPlanByPlan = node["constrainPlanByPlan"].FirstChild.Value == "True";
            //TODO support plans
            constraints.constrainDesign = node["constrainDesign"].FirstChild.Value == "True";
            constraints.texturePackXML = node["texturePackXML"].FirstChild.Value;

            constraints.openingMinimumWidth = float.Parse(node["openingMinimumWidth"].FirstChild.Value);
            constraints.openingMaximumWidth = float.Parse(node["openingMaximumWidth"].FirstChild.Value);
            constraints.openingMinimumHeight = float.Parse(node["openingMinimumHeight"].FirstChild.Value);
            constraints.openingMaximumHeight = float.Parse(node["openingMaximumHeight"].FirstChild.Value);
            constraints.minimumBayMinimumWidth = float.Parse(node["minimumBayMinimumWidth"].FirstChild.Value);
            constraints.minimumBayMaximumWidth = float.Parse(node["minimumBayMaximumWidth"].FirstChild.Value);
            constraints.openingMinimumDepth = float.Parse(node["openingMinimumDepth"].FirstChild.Value);
            constraints.openingMaximumDepth = float.Parse(node["openingMaximumDepth"].FirstChild.Value);
            constraints.facadeMinimumDepth = float.Parse(node["facadeMinimumDepth"].FirstChild.Value);
            constraints.facadeMaximumDepth = float.Parse(node["facadeMaximumDepth"].FirstChild.Value);

            constraints.minimumRoofHeight = float.Parse(node["minimumRoofHeight"].FirstChild.Value);
            constraints.maximumRoofHeight = float.Parse(node["maximumRoofHeight"].FirstChild.Value);
            constraints.minimumRoofDepth = float.Parse(node["minimumRoofDepth"].FirstChild.Value);
            constraints.maximumRoofDepth = float.Parse(node["maximumRoofDepth"].FirstChild.Value);
            constraints.minimumRoofFloorDepth = float.Parse(node["minimumRoofFloorDepth"].FirstChild.Value);
            constraints.maximumRoofFloorDepth = float.Parse(node["maximumRoofFloorDepth"].FirstChild.Value);
            constraints.roofStyleFlat = node["roofStyleFlat"].FirstChild.Value == "True";
            constraints.roofStyleMansard = node["roofStyleMansard"].FirstChild.Value == "True";
            constraints.roofStyleBarrel = node["roofStyleBarrel"].FirstChild.Value == "True";
            constraints.roofStyleGabled = node["roofStyleGabled"].FirstChild.Value == "True";
            constraints.roofStyleHipped = node["roofStyleHipped"].FirstChild.Value == "True";
            constraints.roofStyleLeanto = node["roofStyleLeanto"].FirstChild.Value == "True";
            constraints.roofStyleSteepled = node["roofStyleSteepled"].FirstChild.Value == "True";
            constraints.roofStyleSawtooth = node["roofStyleSawtooth"].FirstChild.Value == "True";
            constraints.allowParapet = node["allowParapet"].FirstChild.Value == "True";
            constraints.parapetChance = float.Parse(node["parapetChance"].FirstChild.Value);
            constraints.allowDormers = node["allowDormers"].FirstChild.Value == "True";
            constraints.dormerChance = float.Parse(node["dormerChance"].FirstChild.Value);
            constraints.dormerMinimumWidth = float.Parse(node["dormerMinimumWidth"].FirstChild.Value);
            constraints.dormerMaximumWidth = float.Parse(node["dormerMaximumWidth"].FirstChild.Value);
            constraints.dormerMinimumHeight = float.Parse(node["dormerMinimumHeight"].FirstChild.Value);
            constraints.dormerMaximumHeight = float.Parse(node["dormerMaximumHeight"].FirstChild.Value);
            constraints.dormerMinimumRoofHeight = float.Parse(node["dormerMinimumRoofHeight"].FirstChild.Value);
            constraints.dormerMaximumRoofHeight = float.Parse(node["dormerMaximumRoofHeight"].FirstChild.Value);
            constraints.dormerMinimumSpacing = float.Parse(node["dormerMinimumSpacing"].FirstChild.Value);
            constraints.dormerMaximumSpacing = float.Parse(node["dormerMaximumSpacing"].FirstChild.Value);


            constraints.parapetMinimumDesignWidth = float.Parse(node["parapetMinimumDesignWidth"].FirstChild.Value);
            constraints.parapetMaximumDesignWidth = float.Parse(node["parapetMaximumDesignWidth"].FirstChild.Value);
            constraints.parapetMinimumHeight = float.Parse(node["parapetMinimumHeight"].FirstChild.Value);
            constraints.parapetMaximumHeight = float.Parse(node["parapetMaximumHeight"].FirstChild.Value);
            constraints.parapetMinimumFrontDepth = float.Parse(node["parapetMinimumFrontDepth"].FirstChild.Value);
            constraints.parapetMaximumFrontDepth = float.Parse(node["parapetMaximumFrontDepth"].FirstChild.Value);
            constraints.parapetMinimumBackDepth = float.Parse(node["parapetMinimumBackDepth"].FirstChild.Value);
            constraints.parapetMaximumBackDepth = float.Parse(node["parapetMaximumBackDepth"].FirstChild.Value);

            constraints.rowStyled = node["rowStyled"].FirstChild.Value == "True";
            constraints.columnStyled = node["columnStyled"].FirstChild.Value == "True";
            constraints.externalAirConUnits = node["externalAirConUnits"].FirstChild.Value == "True";
            constraints.splitLevel = node["splitLevel"].FirstChild.Value == "True";
            constraints.taperedLevels = node["taperedLevels"].FirstChild.Value == "True";
            constraints.singleLevel = node["singleLevel"].FirstChild.Value == "True";
            constraints.atticDesign = node["atticDesign"].FirstChild.Value == "True";
            constraints.shopGroundFloor = node["shopGroundFloor"].FirstChild.Value == "True";

            return constraints;

        }
    }
}
