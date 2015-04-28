// Copyright (c) 2007-2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("icu.net")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SIL International")]
[assembly: AssemblyProduct("icu.net")]
[assembly: AssemblyCopyright("Copyright © SIL International 2007-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3439151b-347b-4321-9f2f-d5fa28b46477")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]
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
#else
#error We need to update the code for newer version of ICU after 56 (or older version before 4.8)
#endif
