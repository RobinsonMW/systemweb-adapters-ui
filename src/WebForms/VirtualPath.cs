// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web;

internal sealed class VirtualPath
{
    public VirtualPath Parent => Directory.GetParent(Path)!.FullName;

    public VirtualPath(string path)
    {
        Path = path;
    }

    public string Path { get; }
    public string VirtualPathStringNoTrailingSlash { get; internal set; }
    public string VirtualPathString => Path;

    public static implicit operator VirtualPath(string path) => new(path);
    public static implicit operator string(VirtualPath vpath) => vpath.Path;

    public static VirtualPath CreateAllowNull(string path) => new(path);

    internal static VirtualPath CreateNonRelativeAllowNull(string v) => v;

    internal static string GetAppRelativeVirtualPathStringOrEmpty(VirtualPath vpath)
        => vpath.Path;

    internal static string GetVirtualPathString(VirtualPath vpath)
        => vpath.Path;

    internal string GetAppRelativeVirtualPathString(VirtualPath templateControlVirtualPath)
    {
        throw new NotImplementedException();
    }

    internal VirtualPath CreateNonRelative(string value)
    {
        throw new NotImplementedException();
    }
}

internal static class VirtualPathProvider
{
    internal static VirtualPath CombineVirtualPathsInternal(VirtualPath templateControlVirtualPath, VirtualPath masterPageFile)
    {
        throw new NotImplementedException();
    }
}
