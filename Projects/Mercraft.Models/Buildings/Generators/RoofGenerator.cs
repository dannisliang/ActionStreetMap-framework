using Mercraft.Models.Buildings.Builders.Roofs;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Generators
{
    public class RoofGenerator
    {
        public static void Generate(Model model, DynamicMultiMaterialMesh mesh)
        {
            Plan plan = model.Plan;

            int numberOfVolumes = model.Plan.Volumes.Count;
            for (int s = 0; s < numberOfVolumes; s++)
            {
                Volume volume = plan.Volumes[s];
                Roof design = model.Roofs[volume.RoofDesignId];

                var roofStyle = design.Style;
                if (volume.Points.Count != 4)
                    if (design.Style == RoofStyle.Leanto || design.Style == RoofStyle.Sawtooth ||
                        design.Style == RoofStyle.Barrel)
                        roofStyle = RoofStyle.Flat;

                if (volume.Points.Count != 4 && design.Style == RoofStyle.Gabled)
                    roofStyle = RoofStyle.Hipped; //ignore style and just do a flat roof

                var builder = GetRoofBuilder(roofStyle, model, mesh);

                Debug.Log("Use builder:" + builder.ToString());

                builder.Build(volume, design);

                //if (design.Parapet)
                    builder.AddParapet(volume, design);
            }
        }

        private static RoofBuilder GetRoofBuilder(RoofStyle roofStyle, Model model,
            DynamicMultiMaterialMesh mesh)
        {
            switch (roofStyle)
            {
                case RoofStyle.Mansard: // something wrong
                    return new MansardRoofBuilder(model, mesh);
                case RoofStyle.Gabled:
                    return new GabledRoofBuilder(model, mesh);
                case RoofStyle.Hipped:
                    return new HippedRoofBuilder(model, mesh);
                case RoofStyle.Leanto: // something wrong
                    return new LeantoRoofBuilder(model, mesh);
                case RoofStyle.Sawtooth: // something wrong
                    return new SawtoothRoofBuilder(model, mesh);
                case RoofStyle.Barrel: // something wrong
                    return new BarrelRoofBuilder(model, mesh);
                case RoofStyle.Steepled:
                    return new SteepledRoofBuilder(model, mesh);
            }
            return new FlatRoofBuilder(model, mesh);
        }
    }
}