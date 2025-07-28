﻿using System;
using System.Collections.Generic;
using System.Linq;
using TDIE.Components.NodeManager.ComponentHost.Data.Entities;

namespace TDIE.Components.NodeManager.Computations
{
    public static class StatsExtensions
    {
        public static double Evenness(this IEnumerable<ComponentHostInstanceSettings> components)
        {
            var sampleSize = components.Count();
            var componentTypeGroups = components.GroupBy(x => x.PackageName).ToDictionary(x => x.Key, x => x.ToArray()); // group by type

            var shannonDiversityIndex = componentTypeGroups
                                                .Select(x => (x.Key, (x.Value.Length / (double)sampleSize)))
                                                .Select(x => x.Item2 * Math.Log(x.Item2))
                                                .Sum() * -1;

            // the closer to one this value is the more diverse our distribution
            double systemEvenness = shannonDiversityIndex / Math.Log(componentTypeGroups.Count); 

            return systemEvenness;
        }      
    }
}
