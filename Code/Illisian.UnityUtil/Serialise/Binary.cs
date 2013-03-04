using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Illisian.UnityUtil.Serialise
{
	/// <summary>
	/// 
	/// </summary>
	public class Binary
	{
		/// <summary>
		/// Bytes to struct.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="location">The location.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static object ByteToStruct(byte[] buffer, int location, Type type)
		{
            if (!type.IsValueType)
                throw new Exception("Type to convert the ByteArray to is not a value type therefore not a struct.");

			int size = Marshal.SizeOf(type);
			IntPtr hd_prt = Marshal.AllocCoTaskMem(size); // Allocates memory for the task
			Marshal.Copy(buffer, location, hd_prt, size); // copy's 16 bytes of the buffer into the pointer
			object objResult = Marshal.PtrToStructure(hd_prt, type); //Maps a structure to the pointer
			Marshal.FreeCoTaskMem(hd_prt); //Frees the Memory Used for the task
			return objResult;

		}
		/// <summary>
		/// Bytes to struct.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="location">The location.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static object ByteToStruct(byte[] buffer, uint location, Type type)
		{
			return ByteToStruct(buffer, (int)location, type);
		}


		/// <summary>
		/// Bytes the array to object.
		/// </summary>
		/// <param name="Buffer">The buffer.</param>
		/// <returns></returns>
		public static object ByteArrayToObject(Byte[] Buffer)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream fs = new MemoryStream(Buffer);
			try
			{
				return formatter.Deserialize(fs);
			}
			catch (SerializationException ex)
			{
				Logging.LogManager.Context.Log(Logging.LogType.Critical, "ByteArrayToObject Failed", ex, Buffer);
				return null;
			}
			finally
			{
				fs.Close();
			}
		}

		/// <summary>
		/// Objects to byte array.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj.GetType().IsSerializable)
            {
                MemoryStream fs = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, obj);
                    return fs.ToArray();
                }
                catch (SerializationException ex)
                {
					Logging.LogManager.Context.Log(Logging.LogType.Critical, "ObjectToByteArray Failed", ex, obj);
                }
                finally
                {
                    fs.Close();
                }
            }
            return null;
        }
		/// <summary>
		/// 
		/// </summary>
		public class Generics
		{
			/// <summary>
			/// Bytes to struct.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="buffer">The buffer.</param>
			/// <returns></returns>
            public static T ByteToStruct<T>(byte[] buffer)
            {
                return ByteToStruct<T>(buffer, 0);
            }

			/// <summary>
			/// Bytes to struct.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="buffer">The buffer.</param>
			/// <param name="location">The location.</param>
			/// <returns></returns>
			public static T ByteToStruct<T>(byte[] buffer, uint location)
			{
				return ByteToStruct<T>(buffer, (int)location);
			}

			/// <summary>
			/// Bytes to struct.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="buffer">The buffer.</param>
			/// <param name="location">The location.</param>
			/// <returns></returns>
			public static T ByteToStruct<T>(byte[] buffer, int location)
			{
                if (!typeof(T).IsValueType)
                    throw new Exception("Type to convert the ByteArray to is not a value type therefore not a struct.");
				int size = Marshal.SizeOf(typeof(T));
				IntPtr hd_prt = Marshal.AllocCoTaskMem(size); // Allocates memory for the task
				Marshal.Copy(buffer, location, hd_prt, size); // copy's 16 bytes of the buffer into the pointer
				object objResult = Marshal.PtrToStructure(hd_prt, typeof(T)); //Maps a structure to the pointer
				Marshal.FreeCoTaskMem(hd_prt); //Frees the Memory Used for the task
				return (T)objResult;

			}
			/// <summary>
			/// Bytes the array to object.
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="Buffer">The buffer.</param>
			/// <returns></returns>
            public static T ByteArrayToObject<T>(Byte[] Buffer)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream fs = new MemoryStream(Buffer);
                try
                {
                    return (T)formatter.Deserialize(fs);
                }
                catch (SerializationException ex)
                {
					Logging.LogManager.Context.Log(Logging.LogType.Critical, "ByteArrayToObject<T> Failed", ex, Buffer);
                    return default(T);
                }
                finally
                {
                    fs.Close();
                }
            }
		}
	}
}
