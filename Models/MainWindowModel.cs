using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Models
{
    class MainWindowModel
    {
        //計算機の結果部分に表示されるstring型の数字を入れるプロパティ
        public string Result { get; private set; }

        // 数字を入力した時に、計算記号、=、ACを押されるまで一時的にそれを格納しておくプロパティ
        public static double KeepNum { get; set; } = 0;

        //　数字を入力された後に計算記号を押すと初めてKeepNumに一時保管されてた数字がKeepNumsリストに追加されて計算用オペランドとして確定する
        public static IList<double> KeepNums { get; set; } = new List<double>();

        public static IList<Func<double, double, double>> Cals { get; set; } = new List<Func<double, double, double>>();


        public static Func<double, double, double> Cal;

        //小数点が１回入力されたらtrueにして小数点を２回以上押せないようにする
        //計算記号かACを押さなければfalseにならないようにする
        public static bool PointCheck { get; set; }


        //　CalCheck  計算ボタンが押されたことを表すプロパティ
        // 　計算ボタンが押されるとこのプロパティをtrueにして、PushコマンドとかではこのCalCheckがtrueになってるかどうかを使って
        //   
        public static bool CalCheck { get; set; } = false;

        // Equal  =が押されるとこのプロパティをtrueにして、PushコマンドではこのCalCheckがtrueになってるかどうかを使って
        //もしtrueの場合は、ViewModelとModelのResultプロパティをnullにしてからfalseにするif文がある
        public static bool EqualCheck { get; set; } = false;


        public void SetText(string str)
        {
            if ((str.Equals("0") || str.Equals("00"))
             && string.IsNullOrEmpty(Result))
            {
                Result = "0";
                return;
            }
            Result += str;
        }

        public void SetResultNull()
        {
            Result = null;
        }

        public double ConvertResultDouble()
        {
            if (Result.Contains(".")) return double.Parse($"{Result}0");
            return double.Parse(Result);
        }

        public void ConvertResult(double a)
        {
            Result = a.ToString();
        }



    }
}
