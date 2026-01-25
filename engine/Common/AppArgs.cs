using CommandLine;

namespace LutViewer.Engine.Common;

public class AppArgs
{
    [Option('i', "image", Required = true, HelpText = "The Image on which the lut will be applyed")]
    public required string ImagePath { get; init; }

    [Option(
        'o',
        "out",
        Required = false,
        Default = "./output",
        HelpText = "The processed images will be saved into this folder"
    )]
    public required string OutputPath { get; init; }

    [Option(
        's',
        "size",
        Required = false,
        Default = -1,
        HelpText = "This specifies the width of the generated previews. Smaller numbers will increase speed. Use -1 to use original size."
    )]
    public required int ProcessSize { get; init; }

    [Option(
        'l',
        "luts",
        Required = true,
        HelpText = "This specifies the folder where all the luts will be stored."
    )]
    public required string LutsPath { get; init; }
}
