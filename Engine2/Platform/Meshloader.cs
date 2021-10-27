using Engine2.Primitives;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2.Platform
{
    class Meshloader
    {

        static Face[] Reduce(Face[] source)
        {
            //Todo implement polygon reduction
            return source;
        }


        public static GameObject[] LoadBabylonFile(string Path)
        {
            var rootobj = JsonConvert.DeserializeObject<Engine2.Platform.Internal.RootObject>(File.ReadAllText(Path));
            var meshes = new List<GameObject>();
           
            foreach(var item in rootobj.meshes)
            {
                var normalArray = item.normals;
                var verticesArray = item.positions;
                var indicesArray = item.indices;

                if (verticesArray == null) continue;
                if (normalArray == null) continue;

                var mesh = new GameObject(item.name, item.positions.Length / 3, item.indices.Length / 3);

                // Filling the Vertices array of our mesh first
                for (var index = 0; index < item.positions.Length - 2; index+=3)
                {
                    var x = (float)verticesArray[index + 0];
                    var y = (float)verticesArray[index + 1];
                    var z = (float)verticesArray[index + 2];

                    var nx = (float)normalArray[index + 0];
                    var ny = (float)normalArray[index + 1];
                    var nz = (float)normalArray[index + 2];

                    mesh.Vertices[index / 3] = new Vertex()
                    {
                        Coordinates = new Vector3(x, y, z),
                        Normal = new Vector3(nx, ny, nz)
                    };
                }

                // Then filling the Faces array
                for (var index = 0; index < item.indices.Length - 2; index+=3)
                {
                    var a = (int)indicesArray[index + 0];
                    var b = (int)indicesArray[index + 1];
                    var c = (int)indicesArray[index + 2];
                    mesh.Faces[index / 3] = new Face { A = a, B = b, C = c };
                }

                // Getting the position you've set in Blender
                var position = item.position;
                //mesh.Position = new Vector3((float)position[0], (float)position[1], (float)position[2]);

                mesh.Faces = Reduce(mesh.Faces);

                mesh.CreateBoundingbox();
                meshes.Add(mesh);
            }
            return meshes.ToArray();
        }
    }
}
