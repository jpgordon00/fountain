using System.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace fountain
{

    /*
    static class CliCommandCollectionExtensions
{
    public static IServiceCollection AddCliCommands(this IServiceCollection services)
    {
        Type greetCommandType = typeof(DeployCommand);
        Type commandType = typeof(Command);

        IEnumerable<Type> commands = greetCommandType
            .Assembly
            .GetExportedTypes()
            .Where(x => x.Namespace == greetCommandType.Namespace && commandType.IsAssignableFrom(x));

        foreach (Type command in commands)
        {
            services.AddSingleton(commandType, command);
        }

        services.AddSingleton(sp =>
        {
            return
               sp.GetRequiredService<IConfiguration>().GetSection("Deployment").Get<DeploymentOptions>()
               ?? throw new ArgumentException("Deployment configuration cannot be missing.");
        });

        return services;
    }
}
*/

    /// <summary>
    /// An attempt to allow for implicit child class discovery. Unfortunately, I was unable
    /// to achieve this with C#'s Assembly interface without each child being explicitly
    /// referenced elsewhere in the source code (somewhere that is directly invoked by the Program) 
    /// </summary>
    public static class CommandFactory
    {


        
        /// <summary>
        /// Stores all the 'Registry' children objects.
        /// </summary>
        /// <typeparam name="Registry"></typeparam>
        /// <returns></returns>
        public static List < Type > _Types = new List < Type > ();
        public static List < Command > Commands = new List < Command > ();

        /// <summary>
        /// Searches through assembly to find all children of 'IDownloadFulfiller' and store it in '_Types'
        /// </summary>
        static CommandFactory()
        {
            _Types = typeof (Command).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof (Command)) && !t.IsAbstract).Select(t => {
                //if (t == typeof (RootCommand)) return null; // ignore base types
                try
                {
                    Commands.Add((Command) Activator.CreateInstance(t));
                } catch (Exception ex) {}
                return t; //(IDownloadFulfiller) Activator.CreateInstance(t);
            }).ToList();
        }

        /// <summary>
        /// Given a string matching the complete classname of some 'Registry' child, return a new instance of that class.
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static Command CreateFromClassName(string classname) {
            return (Command) Activator.CreateInstance(_Types.Find(t => t.Name == classname));
        }
    }
}