/**
Copyright 2021 kaede (polaris)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


*/

namespace ps1exe {
  using System;
  using System.Windows.Forms;
  using System.Management.Automation;

  public class Base : Form {
    private void Fload(object sender, EventArgs e){
      string[] CStr = System.Environment.GetCommandLineArgs();
      using (RunspaceInvoke invoker = new RunspaceInvoke()) {
        invoker.Invoke(psscript,CStr);
      }
      this.Close();
    }
    public Base(){
      this.Load += this.Fload;
    }
    [STAThread]
    public static void Main() {
      Application.Run(new Base());
    }
    private string psscript = @"
# region PowerShell Scrpit
# Escape double-quotation in this variable.

using namespace System.Windows.Forms;
Add-Type -AssemblyName System.Windows.Forms;
Add-Type -AssemblyName System.Drawing;
 
[Application]::EnableVisualStyles();
[Form1]::main();

Class Form1 :Form{
    $table1 = [form1table]::new();

    Form1(){
        $this.text = ""Polaris抽選ツール""
        $this.size = [Drawing.Size]::new(720,480);
        $this.BackColor = ""#FFFFFF""
        $this.table1.btn.add_Click({$This.FindForm().table1.lottey()});

        $this.Controls.addRange(
            @(
                $this.table1
            )
        );
    }

    static [int] Main(){
        $lottool =[Form1]::new()
        $lottool.table1.timer.add_tick({$lottool.table1.timelot()});
        $lottool.ShowDialog();
        return 1;
    }
}

Class form1table : TableLayoutPanel{
    $Selectbox = [Textbox]::new();
    $time = [FlowLayoutPanel]::new();
    $btn = [Button]::new();
    $timebox = [NumericUpDown]::new();
    $output = [label]::new();
    $result = [label]::new();
    $logbox = [Textbox]::new();
    $timer = [timer]::new();

    static [int]$tick = 0

    form1table(){
        $this.Timer.stop();
        $this.timer.Interval  = 100;
        $this.ColumnCount = 2;
        $this.RowCount = 4;
        $this.ColumnStyles.Add([ColumnStyle]::new([SizeType]::Percent,50));
        $this.ColumnStyles.Add([ColumnStyle]::new([SizeType]::Percent,50));
        $this.RowStyles.Add([ColumnStyle]::new([SizeType]::Percent,8));
        $this.RowStyles.Add([ColumnStyle]::new([SizeType]::Percent,74));
        $this.RowStyles.Add([ColumnStyle]::new([SizeType]::Percent,8));
        $this.RowStyles.Add([ColumnStyle]::new([SizeType]::Percent,10));
        $this.Dock = [DockStyle]::Fill;
        
        $this.s_selectbox();
        $this.s_time();
        $this.s_btn();
        $this.s_output();
        $this.s_logbox();
    }

    [void] s_selectbox(){
        $this.Selectbox.Dock = [DockStyle]::Fill;
        $this.Selectbox.Multiline = $true;
        $this.Selectbox.ScrollBars = ""Vertical"";
        $this.Controls.Add($this.Selectbox, 0, 0);
        $this.SetRowSpan($this.Selectbox, 2)
        
    }

    [void] s_time(){
        $this.time.Dock = [DockStyle]::Fill;
        $this.time.FlowDirection = [FlowDirection]::LeftToRight;
        $this.time.WrapContents = $false;

        $timelabel = [label]::new();
        
        $timelabel2 = [label]::new();

        $timelabel.text = ""結果発表までの時間："";
        $timelabel.size = [Drawing.Size]::new(120,20);
        $timelabel.TextAlign = ""MiddleLeft"";
        $timelabel2.text = ""秒"";
        $timelabel2.TextAlign = ""MiddleLeft"";

        $this.timebox.size = [Drawing.Size]::new(50,20);
        $this.timebox.value = 3;

        $this.time.Controls.AddRange(@($timelabel,$this.timebox,$timelabel2));
        $this.Controls.Add($this.time, 0, 2);
    }

    [void] s_btn(){
        $this.btn.text = ""抽選"";
        $this.btn.Dock = [DockStyle]::Fill;
        $this.Controls.Add($this.btn, 0, 3);
    }

    [void] s_output(){
        $this.output.text = ""選ばれたのは…""
        $this.output.TextAlign = ""MiddleCenter"";
        $this.output.Dock = [DockStyle]::Fill;
        $this.result.text = """"
        $this.result.ForeColor = ""blue""
        $this.result.Font = [Drawing.Font]::new(""Times New Roman"",28)
        $this.result.TextAlign = ""MiddleCenter"";
        $this.result.Dock = [DockStyle]::Fill;
        $this.Controls.Add($this.output, 1, 0);
        $this.Controls.Add($this.result, 1, 1);
    }

    [void] s_logbox(){
        $this.logbox.text = """"
        $this.logbox.Dock = [DockStyle]::Fill;
        $this.logbox.Multiline = $true;
        $this.logbox.ScrollBars = ""Vertical"";
        $this.Controls.Add($this.logbox, 0, 2);
        $this.SetRowSpan($this.logbox, 2)
    }

    timelot(){
        [int]$s = ([int]$this.timebox.text)*10;
        [form1table]::tick += 1;
        if($this.Selectbox.Text -eq """"){
            $i = Get-Random 100;
        }
        else{
            $i = $this.Selectbox.Text.Split(""`r`n"")|Get-Random;
        };
        $this.result.text = $i;
        if($s -le [form1table]::tick){
            $this.Timer.dispose();
            $this.logbox.text += $this.result.text+""`r`n"";
        };
    }

    lottey(){
        [form1table]::tick = 0;
        $this.Timer.start();
    }
}

# endregion
";
  }
}