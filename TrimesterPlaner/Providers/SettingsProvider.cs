﻿using TrimesterPlaner.Models;
using Utilities.Providers;

namespace TrimesterPlaner.Providers
{
    public interface ISettingsProvider : ISettingsProviderBase<Settings>
    { }

    public class SettingsProvider : SettingsProviderBase<Settings>, ISettingsProvider
    { }
}