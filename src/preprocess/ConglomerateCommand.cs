using System.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace fountain
{
    /// <summary>
    /// Handles the conglomeration option for printing source code.
    /// </summary>
    public class ConglomerateCommand : CommandExt
    {

        public Action<string> OnHandle;

        /// <summary>
        /// For each source file that is processed, append this strend and replace the following:
        ///     FILE_NAME with the full file name( ex: img.jpg )
        ///     FILE_CONTENTS is the full files contents
        /// </summary>
        protected const string SOURCE_DELIMITER = "/snf\\\\in a file named FILE_NAME...\nFILE_CONTENTS";

        internal bool _b = false;

        /// <summary>
        /// Sets this commands handler and adds the following options:
        ///     srcdir
        ///     excludedirs
        ///     includetypes
        /// </summary>
        /// <returns></returns>
        public ConglomerateCommand() : base("conglomerate")
        {
            var srcDirOption = new Option<string>
            (
                name: "--srcdir",
                description: "Directory that the source code will be read from",
                getDefaultValue: () => null
            );
            var excludeDirectoryNamesOptions = new Option<string>
            (
                name: "--excludedirs",
                description: "Comma seperated list of directory names to exclude",
                getDefaultValue: () => "nodemodules, node_modules, utils"
            );
            var includeTypesOptions = new Option<string>
            (
                name: "--includetypes",
                description: "Comma seperated list of types to include only.",
                getDefaultValue: () => ".cs, .js, .cpp, .py, .php"
            );
            var stripWhitespaces = new Option<bool>
            (
                name: "--strip",
                description: "Strips the src of all whitespaces",
                getDefaultValue: () => true
            );
            Add(srcDirOption);
            Add(excludeDirectoryNamesOptions);
            Add(includeTypesOptions);

            // use 'FileUtils.ReadAllFilesRecursively', and 'StringUtils.ParseCommaOrSpaceSeperatedString'.
            // also adhere to exclusion directories and inclusion types
            this.SetHandler(
                (srcDir, excludeDirectoryStr, includeTypesStr) =>
                {
                    if (srcDir == null) srcDir = Directory.GetCurrentDirectory();
                    string[] excludeDirectoryNames = StringUtils.ParseCommaOrSpaceSeperatedString(excludeDirectoryStr);
                    string[] includeTypes = StringUtils.ParseCommaOrSpaceSeperatedString(includeTypesStr);
                    string str = "";
                    FileUtils.ReadAllFilesRecursively(srcDir, excludeDirectoryNames, includeTypes, (path, name, contents) => {
                        // explicitly look for file breaks when attempting to use /sn
                        if (contents.Contains("\n"))
                        {
                            string _str = "";
                            var lines = contents.Split("\n");
                            foreach (var line in lines)
                            {
                                string v = line.Trim();
                                if (v == "") continue; // case: empty line, skip
                                _str += v;
                            }
                            if (_str == "") return; // case: empty file
                            contents = _str;
                        }
                        str += $"\n//in a file named '{name}': {contents}\n";
                    }, () => {
                        str += "\n//end of src code";
                        Console.WriteLine(str);
                        OnHandle?.Invoke(str);
                    });
                },
            srcDirOption, excludeDirectoryNamesOptions, includeTypesOptions);
        }
        
    }
}