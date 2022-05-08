using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DialogOption
{
    public string text;
    public int nextDialog;
    public int interact = 0;
    public DialogResult result;

    public DialogOption(int currentScene)
    {
        text = "";
        nextDialog = -1;
        result = DialogResult.END;
    }

    public DialogOption(int currentScene, string _text, string _result, string other)
    {
        text = _text;
        switch (_result)
        {
            case "PROGRESS":
                result = DialogResult.PROGRESS;
                nextDialog = currentScene + 1;
                return;
            case "END":
                result = DialogResult.END;
                nextDialog = -1;
                return;
            case "INTERACT":
                result = DialogResult.INTERACT;
                nextDialog = currentScene + 1;
                interact = int.Parse(other);
                return;
            case "TRANSFER":
                result = DialogResult.TRANSFER;
                nextDialog = int.Parse(other) + 1;
                return;
            default:
                result = DialogResult.END;
                nextDialog = -1;
                return;
        }
    }

    public string GetTypeText()
    {
        switch (result)
        {
            case DialogResult.PROGRESS:
                return "PROGRESS";
            case DialogResult.TRANSFER:
                return "TRANSFER";
            case DialogResult.INTERACT:
                return "INTERACT";
            case DialogResult.END:
                return "END";
            default:
                return "";
        }
    }
}
