using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessRepertoire.Model.Variation;

namespace ChessRepertoire.Infrastructure.Repository {
    public class VariationGraphRepository : IVariationGraphRepository {
        public VariationGraph GetGraph() {
            var graph = new VariationGraph();

            // Get all *.txt files in the specified folder
            var files = Directory.GetFiles("repertoire/Caro Kann Defense", "*.txt");

            foreach (var filePath in files) {
                Console.WriteLine($"Processing file: {filePath}");

                // Open the file for reading
                using (var reader = new StreamReader(filePath)) {
                    // Iterate over the lines in the file
                    while (reader.ReadLine() is { } line) {
                        Console.WriteLine(line);
                        // Perform any processing on the line here
                    }
                }

                Console.WriteLine($"Finished processing file: {filePath}");
            }

            return graph;
        }
    }
}
