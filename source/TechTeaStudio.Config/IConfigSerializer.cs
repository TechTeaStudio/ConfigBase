namespace TechTeaStudio.Config
{
	public interface IConfigSerializer<T>
	{
		T Deserialize(string content);
		string Serialize(T config);
	}

}
