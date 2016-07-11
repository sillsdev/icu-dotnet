// Copyright (c) 2016 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.Reflection;

// We add these AssemblyInformationVersion numbers in a separate file so that the
// StampAssemblies task that gets run during a command line build doesn't change the
// numbers.
#if ICU_VER_40
[assembly: AssemblyInformationalVersion("4.0.0.0")]
#elif ICU_VER_48
[assembly: AssemblyInformationalVersion("4.8.0.0")]
#elif ICU_VER_49
[assembly: AssemblyInformationalVersion("49.0.0.0")]
#elif ICU_VER_50
[assembly: AssemblyInformationalVersion("50.0.0.0")]
#elif ICU_VER_51
[assembly: AssemblyInformationalVersion("51.0.0.0")]
#elif ICU_VER_52
[assembly: AssemblyInformationalVersion("52.0.0.0")]
#elif ICU_VER_53
[assembly: AssemblyInformationalVersion("53.0.0.0")]
#elif ICU_VER_54
[assembly: AssemblyInformationalVersion("54.0.0.0")]
#elif ICU_VER_55
[assembly: AssemblyInformationalVersion("55.0.0.0")]
#elif ICU_VER_56
[assembly: AssemblyInformationalVersion("56.0.0.0")]
#elif ICU_VER_57
[assembly: AssemblyInformationalVersion("57.0.0.0")]
#elif ICU_VER_58
[assembly: AssemblyInformationalVersion("58.0.0.0")]
#elif ICU_VER_59
[assembly: AssemblyInformationalVersion("59.0.0.0")]
#elif ICU_VER_60
[assembly: AssemblyInformationalVersion("60.0.0.0")]
#else
#error We need to update the code for newer version of ICU after 60 (or older version before 4.8)
#endif
