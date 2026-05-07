
namespace MultiDbClientIDE.Properties
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class Resources
	{
		private static System.Resources.ResourceManager resourceMan;

		private static object internalSyncObject = new object();

		internal Resources()
		{
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if ((resourceMan == null))
				{
					lock (internalSyncObject)
					{
						if (resourceMan == null)
						{
							var temp = new System.Resources.ResourceManager("MultiDbClientIDE.Properties.Resources", typeof(Resources).Assembly);
							resourceMan = temp;
						}
					}
				}
				return resourceMan;
			}
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static System.Globalization.CultureInfo Culture
		{
			get { return null; }
			set { }
		}
	}
}
