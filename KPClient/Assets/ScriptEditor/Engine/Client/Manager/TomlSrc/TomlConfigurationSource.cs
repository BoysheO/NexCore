using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Debug = UnityEngine.Debug;

namespace BoysheO.Extensions.Configuration.Toml
{
    /// <summary>
    /// Represents a TOML file as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class TomlConfigurationSource : FileConfigurationSource
    {
        /// <summary>
        /// Builds the <see cref="TomlConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>An <see cref="TomlConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            var st = new Stopwatch();
            st.Start();
            try
            {
                return new TomlConfigurationProvider(this);
            }
            finally
            {
                if (st.Elapsed > TimeSpan.FromSeconds(10))
                {
                    Debug.Log($"TomlConfiguration init too long-{st.Elapsed},it will appear on some os system while FileConfigurationSource using reloadOnChange is on.Please turn reloadOnChanged off to avoid this issue.");
                }
            }
        }
    }
}