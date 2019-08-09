using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
/*
 * 单一字符提取器
 * 将目标文件中的字符扫描并将每种字符只提取一个，最后按ASCII进行排序;
 * 可设置忽略字符，默认的勾选忽略空格，将忽略 所有空格与换行符。并可自行添加所想忽略的字符
 */
namespace GetSingChar
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private string lastFileSavePath = "";
        private bool isIgnoeSpace = true;
        public List<char> ignoeChars = new List<char>() { ' ', '　', '\n'};
        List<char> addIgnoe = new List<char>();

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = filePathText.Text;
            string savePath = fileSavePath.Text;

            string[] filesPath = null;

            isIgnoeSpace = (bool)ignoeToggle.IsChecked;
            if (fileOrfolder.SelectedIndex == 0)
            {

                if (!File.Exists(filePath))
                {
                    navLabel.Content = "源文件出错不存在该文件";
                    filePathText.Text = "";
                    return;
                }
                else
                {
                    filesPath = new string[] { filePath };
                }
            }
            else if (fileOrfolder.SelectedIndex == 1)
            {

                if (!Directory.Exists(filePath))
                {
                    navLabel.Content = "文件夹地址出错不存在该文件夹地址";
                    filePathText.Text = "";
                    return;
                }
                else {

                    filesPath = Directory.GetFiles(filePath, "*.txt");
                }
            }
            if (filesPath == null)
            {
                navLabel.Content = "文件夹地址出错不存在该文件夹地址";
                filePathText.Text = "";
                return;
            }
            bool isCursSavePath = !(bool)saveToggle.IsChecked;
            string saveFolder = "";

            if (isCursSavePath)
            {
                if (!Directory.Exists(savePath))
                {
                    navLabel.Content = "保存目录出错不存在该目录";
                    fileSavePath.Text = "";
                    return;
                }
                else
                {
                    saveFolder = savePath+"\\";
                }
            }
            else
            {
                if (fileOrfolder.SelectedIndex == 0)
                {
                    string[] _name = filePath.Split('\\');

                    for (int i = 0; i < _name.Length-1; i++)
                    {
                        saveFolder += _name[i] + "\\";
                    }
                }
                else
                {
                    saveFolder = filePath + "\\";
                }
            }
            for (int i = 0; i < filesPath.Length; i++)
            {
                string[] spli = filesPath[i].Split('\\');
                string _name = spli[spli.Length - 1];

                GetNeedText(filesPath[i], saveFolder + "only_" + _name,i+1,filesPath.Length);
            }
        }

        public void GetNeedText(string _filePath,string _fileSavePath,int now=1,int count=1)
        {
            List<char> texts = new List<char>();

            FileStream fileStream = new FileStream(_filePath, FileMode.Open);

            byte[] data = new byte[fileStream.Length];

            fileStream.Read(data, 0, data.Length);
            fileStream.Close();

            string str = Encoding.UTF8.GetString(data, 0, data.Length);

            char[] texs = str.ToCharArray();

            if (isIgnoeSpace)
            {
                if (addIgnoe.Count > 0)
                {
                    for (int i = 0; i < texs.Length; i++)
                    {
                        if (!ignoeChars.Contains(texs[i]) && !addIgnoe.Contains(texs[i]) && !texts.Contains(texs[i]))
                        {
                            texts.Add(texs[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < texs.Length; i++)
                    {
                        if (!ignoeChars.Contains(texs[i]) && !texts.Contains(texs[i]))
                        {
                            texts.Add(texs[i]);
                        }
                    }
                }
            }
            else {

                for (int i = 0; i < texs.Length; i++)
                {
                    if (!texts.Contains(texs[i]))
                    {
                        texts.Add(texs[i]);
                    }
                }
            }


            texts.Sort((char a, char b) => {
                if ((int)a < (int)b)
                {
                    return 1;
                }
                else if ((int)a == (int)b)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            });

            StreamWriter streamWriter = new StreamWriter(_fileSavePath, false);
            streamWriter.Write(texts.ToArray());
            streamWriter.Close();
            
            navLabel.Content = "[" + now + "," + count + "]生成成功：[源字数：" + texs.Length + ", 仅有字数：" + texts.Count+"]";

        }
        
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            fileSavePath.IsEnabled = !((bool)saveToggle.IsChecked);

            if (fileSavePath.IsEnabled)
            {
                lastFileSavePath = filePathText.Text;
                string[] strs = filePathText.Text.Split('\\');
                string _temp = "";
                for (int i = 0; i < strs.Length - (fileOrfolder.SelectedIndex == 0 ? 1 :0); i++)
                {
                    _temp += strs[i] + "\\";
                }
                fileSavePath.Text = _temp;
                navLabel.Content =  "请自定义文件保存目录";
            }
            else
            {
                filePathText.Text = lastFileSavePath;
                navLabel.Content = "文件将保存到源文件的目录中";
            }
        }

        private void FilePathText_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;//必须加
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            string msg = "Drop";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                msg = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            }
            filePathText.Text = msg;
            navLabel.Content = "获取文件：" +msg;

            if ((bool)saveToggle.IsChecked) {

                lastFileSavePath = filePathText.Text;
                string[] strs = filePathText.Text.Split('\\');
                string _temp = "";
                for (int i = 0; i < strs.Length - (fileOrfolder.SelectedIndex == 0 ? 1 : 0); i++)
                {
                    _temp += strs[i] + "\\";
                }
                fileSavePath.Text = _temp;
                navLabel.Content = "请自定义文件保存目录";
            }
        }

        private void NavLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            navLabel.Content = "";
        }

        private void FileSavePath_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            string msg = "Drop";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                msg = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            }
            fileSavePath.Text = msg;
            navLabel.Content = "文件保存目录：" + msg;
        }

        private void IgnoeText_TextChanged(object sender, TextChangedEventArgs e)
        {
            char[] temp = ignoeText.Text.ToArray();

            addIgnoe.Clear();
            for (int i = 0; i < temp.Length; i++)
            {
                if (!addIgnoe.Contains(temp[i])) {
                    addIgnoe.Add(temp[i]);
                }
            }
        }

        private void IgnoeToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (((bool)ignoeToggle.IsChecked))
            {
                ignoeToggle.Content = "忽略空格";
                ignoeText.Visibility = Visibility.Visible;
            }
            else
            {
                ignoeToggle.Content = "忽略空格+[字符扩充]";
                ignoeText.Visibility = Visibility.Hidden;
            }
        }
    }
}
