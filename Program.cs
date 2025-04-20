using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

public class Program
{
    [STAThread]
    public static void Main()
    {
        Console.Title = "IORewriter | Made by https://github.com/ZygoteCode/";
        selectionStage: List<string> paths = new List<string>();
        bool canContinue = true;

        while (canContinue)
        {
            Console.WriteLine("Choose one of the available options:\r\n\r\n1) Select file\r\n2) Select folder\r\n3) Insert path manually\r\n4) Terminate selection process");
            string chosen = Console.ReadLine();

            if (chosen != "1" && chosen != "2" && chosen != "3" && chosen != "4")
            {
                Console.WriteLine("Invalid answer. Please, try again.");
                continue;
            }

            if (chosen == "1")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Select your file(s)";
                openFileDialog.DefaultExt = Environment.SpecialFolder.Desktop.ToString();
                
                if (openFileDialog.ShowDialog().Equals(DialogResult.OK))
                {
                    paths.AddRange(openFileDialog.FileNames);
                }
            }
            else if (chosen == "2")
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog.ShowNewFolderButton = true;

                if (folderBrowserDialog.ShowDialog().Equals(DialogResult.OK))
                {
                    paths.Add(folderBrowserDialog.SelectedPath);
                }
            }
            else if (chosen == "3")
            {
                string thePath = "";

                while (!File.Exists(thePath) && !Directory.Exists(thePath))
                {
                    Console.Write("Please, enter your path here: ");
                    thePath = Console.ReadLine();

                    if (!File.Exists(thePath) && !Directory.Exists(thePath))
                    {
                        Console.WriteLine("Invalid path (no file or folder found). Please, try again.");
                    }
                }

                paths.Add(thePath);
            }
            else
            {
                canContinue = false;
            }
        }

        if (paths.Count == 0)
        {
            Console.WriteLine("No paths have been selected.");
            Console.WriteLine("Returning to the selection stage.");
            goto selectionStage;
        }

        Console.WriteLine("Rewriting your data, please wait a while.");
        string rewrittenPath = Path.GetFullPath("rewritten_files");

        if (Directory.Exists(rewrittenPath))
        {
            Directory.Delete(rewrittenPath, true);
        }

        Directory.CreateDirectory(rewrittenPath);
        
        foreach (string path in paths)
        {
            Rewrite(path, Path.GetFullPath(rewrittenPath), path);
        }    

        Console.WriteLine("Rewriting process succesfully done.");
        Console.WriteLine("Press the ENTER key in order to exit from the program.");
        Console.ReadLine();
    }

    public static void Rewrite(string toBeRewritePath, string rewrittenPath, string baseToBeRewritePath)
    {
        if (File.Exists(toBeRewritePath))
        {
            RewriteFile(toBeRewritePath, rewrittenPath, baseToBeRewritePath);
        }
        else if (Directory.Exists(toBeRewritePath))
        {
            RewriteDirectory(toBeRewritePath, rewrittenPath, baseToBeRewritePath);
            
            foreach (string thePath in FindFiles(toBeRewritePath))
            {
                if (File.Exists(thePath))
                {
                    RewriteFile(thePath, rewrittenPath, baseToBeRewritePath);
                }
                else if (Directory.Exists(thePath))
                {
                    RewriteDirectory(thePath, rewrittenPath, baseToBeRewritePath);
                }
            }
        }
    }

    public static void RewriteFile(string toBeRewritePath, string rewrittenPath, string baseToBeRewritePath)
    {
        string midPath = toBeRewritePath.Replace(baseToBeRewritePath, "");
        string newRewritePath = $"{rewrittenPath}{midPath}";
        File.WriteAllBytes(newRewritePath, File.ReadAllBytes(toBeRewritePath));
    }

    public static void RewriteDirectory(string toBeRewritePath, string rewrittenPath, string baseToBeRewritePath)
    {
        string midPath = toBeRewritePath.Replace(baseToBeRewritePath, "");
        string newRewritePath = $"{rewrittenPath}{midPath}";
        Directory.CreateDirectory(newRewritePath);
    }

    private static List<string> FindFiles(string path)
    {
        List<string> paths = new List<string>();

        foreach (string filePath in Directory.GetFiles(path))
        {
            paths.Add(filePath);
        }

        foreach (string folderPath in Directory.GetDirectories(path))
        {
            paths.Add(folderPath);
            paths.AddRange(FindFiles(folderPath));
        }

        return paths;
    }
}
