//
// System.Security.SecurityManager.cs
//
// Authors:
//	Nick Drochak(ndrochak@gol.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) Nick Drochak
// Portions (C) 2004 Motus Technologies Inc. (http://www.motus.com)
// Copyright (C) 2004-2005, 2009 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if NET_2_1

using System.Reflection;
using System.Security.Policy;

namespace System.Security {

	// Must match MonoDeclSecurityActions in /mono/metadata/reflection.h
	internal struct RuntimeDeclSecurityActions {
		public RuntimeDeclSecurityEntry cas;
		public RuntimeDeclSecurityEntry noncas;
		public RuntimeDeclSecurityEntry choice;
	}

	internal static class SecurityManager {

		// note: CAS (not CoreCLR) related
		public static bool SecurityEnabled {
			get { return false; }
		}

		internal static IPermission CheckPermissionSet (Assembly a, PermissionSet ps, bool noncas)
		{
			return null;
		}

		internal static IPermission CheckPermissionSet (AppDomain ad, PermissionSet ps)
		{
			return null;
		}

		internal static PermissionSet Decode (byte[] encodedPermissions)
		{
			return null;
		}

		internal static PermissionSet Decode (IntPtr permissions, int length)
		{
			return null;
		}

		public static bool IsGranted (IPermission perm)
		{
			return false;
		}

		public static PermissionSet ResolvePolicy (Evidence evidence)
		{
			return null;
		}

		public static PermissionSet ResolvePolicy (Evidence evidence, PermissionSet reqdPset, PermissionSet optPset, PermissionSet denyPset, out PermissionSet denied)
		{
			denied = null;
			return null;
		}

		internal static bool ResolvePolicyLevel (ref PermissionSet ps, PolicyLevel pl, Evidence evidence)
		{
			return false;;
		}

		internal static PolicyLevel ResolvingPolicyLevel {
			get { return null; }
		}

		internal static void ReflectedLinkDemandInvoke (MethodBase mb)
		{
		}

		// called by the runtime when CoreCLR is enabled

		private static void FieldAccessException (IntPtr caller, IntPtr field)
		{
			throw new FieldAccessException (Locale.GetText ("Field access not allowed."));
		}

		private static void MethodAccessException (IntPtr caller, IntPtr callee)
		{
			throw new MethodAccessException (Locale.GetText ("Method call not allowed."));
		}

		// internal - get called by the class loader

		// Called when
		// - class inheritance
		// - method overrides
		private unsafe static bool InheritanceDemand (AppDomain ad, Assembly a, RuntimeDeclSecurityActions *actions)
		{
			return false;
		}

		private static void InheritanceDemandSecurityException (int securityViolation, Assembly a, Type t, MethodInfo method)
		{
		}

		// internal - get called at JIT time

		private static void DemandUnmanaged ()
		{
		}

		// internal - get called by JIT generated code

		private static void InternalDemand (IntPtr permissions, int length)
		{
		}

		private static void InternalDemandChoice (IntPtr permissions, int length)
		{
		}

		private unsafe static bool LinkDemand (Assembly a, RuntimeDeclSecurityActions *klass, RuntimeDeclSecurityActions *method)
		{
			return false;
		}

		private static bool LinkDemandUnmanaged (Assembly a)
		{
			return false;
		}

		private static bool LinkDemandFullTrust (Assembly a)
		{
			return false;
		}

		private static void LinkDemandSecurityException (int securityViolation, IntPtr methodHandle)
		{
		}
	}
}

#endif
