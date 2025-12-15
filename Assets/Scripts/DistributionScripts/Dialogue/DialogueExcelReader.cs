using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogueExcelReader
{
    public List<DialogueLine> ReadCSV(TextAsset csvFile)
    {
        List<DialogueLine> lines = new List<DialogueLine>();

        if (csvFile == null)
        {
            Debug.LogError("DialogueExcelReader: Geen CSV-bestand meegegeven.");
            return lines;
        }

        using (StringReader reader = new StringReader(csvFile.text))
        {
            string line;
            bool isHeader = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                string[] values = line.Contains(";") ? line.Split(';') : line.Split(',');

                if (values.Length < 3)
                {
                    Debug.LogWarning($"DialogueExcelReader: Regel heeft te weinig kolommen ({values.Length}). Inhoud: {line}");
                    continue;
                }

                DialogueLine d = new DialogueLine();

                int.TryParse(values[0], out d.ID);
                d.Speaker = Clean(values[1]);
                d.Dialogue = Clean(values[2]);

                if (values.Length > 3)
                    d.SoundName = Clean(values[3]);
                else
                    d.SoundName = string.Empty;

                lines.Add(d);
            }
        }

        return lines;
    }

    private string Clean(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.Trim().Trim('"');
    }
}
