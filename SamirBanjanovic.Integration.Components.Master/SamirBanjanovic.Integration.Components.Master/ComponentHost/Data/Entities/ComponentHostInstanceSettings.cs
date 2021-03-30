using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Components.Master.ComponentHost.Data.Entities
{
    public class ComponentHostInstanceSettingsWithPublisher
        : ComponentHostInstanceSettings
        , IEquatable<ComponentHostInstanceSettingsWithPublisher>
    {
        public ComponentHostInstanceSettings MessagePublisher { get; set; }

        public bool Equals(ComponentHostInstanceSettingsWithPublisher other)
        {
            return base.Equals(other) && MessagePublisher.Equals(other.MessagePublisher);
        }
    }


    public class ComponentHostInstanceSettings
        : IEquatable<ComponentHostInstanceSettings>
    {
        public long Id { get; set; }

        public string PackageName { get; set; }

        public string PackageVersion { get; set; }

        public IDictionary<string, string> Settings { get; set; }

        public bool Equals(ComponentHostInstanceSettings other)
        {
            if(other is null)
            {
                return false;
            }

            return Id == other.Id
                && PackageName == other.PackageName
                && PackageVersion == other.PackageVersion;
        }
    }
}
