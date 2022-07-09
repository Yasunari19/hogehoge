using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Regions;
using System;
using Reactive.Bindings;
using Prism.Events;
using System.Windows.Input;
using Calculator.Models;
using System.ComponentModel;
using System.Collections.Generic;


namespace Calculator.ViewModels
{
    public class MainWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        public string Result { get; set; }

        public ICommand Push { get; set; }

        public ICommand Calculate { get; set; }

        public DelegateCommand Equal { get; set; }

        public DelegateCommand Clear { get; set; }

        public DelegateCommand AllClear { get; set; }




        public event PropertyChangedEventHandler PropertyChanged;



        public MainWindowViewModel()
        {
            var model = new MainWindowModel();
            Push = new CustomDelegateCommand(
                param =>
                {




                    // 計算ボタンを押された後の数字入力の場合の処理
                    // 計算方法の仮プロパティCalに代入されてる計算をCalsに追加した後、CalCheckを外す
                    if (MainWindowModel.CalCheck == true)
                    {
                        MainWindowModel.KeepNums.Add(MainWindowModel.KeepNum);
                        MainWindowModel.Cals.Add(MainWindowModel.Cal);
                        MainWindowModel.Cal = null;
                        MainWindowModel.CalCheck = false;
                        MainWindowModel.PointCheck = false;
                        Result = null;
                        model.SetResultNull();
                    }

                    // =を押された後の数字入力の場合の処理
                    if (MainWindowModel.EqualCheck == true)
                    {
                        Result = null;
                        model.SetResultNull();
                        MainWindowModel.EqualCheck = false;
                    }

                    if (param == null) { return; }
                    var str = param.ToString();


                    // 数値入力の時、最初に小数点を押せないようにする
                    if (Result == null && str.Equals(".")) return;

                    // 0始まりの整数を除外するけど、0の後に小数点が入力された場合は条件外とする
                    // Result == nullはアプリ起動時と、=かACを押した直後という意味なので、数値入力の一番最初を表してるため、条件外
                    if (Result == "0")
                    {
                        if (!(str.Equals("."))) return;
                    }

                    // 小数点を2回以上押せないようにする
                    if (MainWindowModel.PointCheck == true && str.Equals(".")) return;

                    // 1回小数点を入力されたらPointCheckをtrueにして、true状態の時に小数点が入力されたらreturnする
                    if (Result != null && str.Equals(".")) MainWindowModel.PointCheck = true;



                    model.SetText(str);

                    MainWindowModel.KeepNum = model.ConvertResultDouble();
                    Result = model.Result;



                    var args = new PropertyChangedEventArgs(nameof(Result));
                    PropertyChanged(this, args);
                },
                param =>
                {
                    return true;
                });

            Calculate = new CustomDelegateCommand
                            (
                                (param) =>
                                {
                                    //　計算ボタンを押す前に数字が入力されてなければボタンクリックは無効
                                    //　Result = nullということはアプリ起動時か=を押された後のどっちかということで、どっちの場合でもCalCheckがfalseになってる
                                    if (Result != null)
                                    {
                                        // =が押された後に続けて計算記号を押されたりしたらreturnする
                                        if (MainWindowModel.EqualCheck == true) return;
                                        switch (param)
                                        {
                                            case "Add":
                                                MainWindowModel.Cal = Add;
                                                break;
                                            case "Sub":
                                                MainWindowModel.Cal = Sub;
                                                break;
                                            case "Mul":
                                                MainWindowModel.Cal = Mul;
                                                break;
                                            case "Div":
                                                MainWindowModel.Cal = Div;
                                                break;
                                            case "Rem":
                                                MainWindowModel.Cal = Rem;
                                                break;
                                            default:
                                                break;
                                        }

                                        
                                        //このif文条件に引っかかるということは既にCalCheckがtrueになってるはず
                                        //数字が入力された上で、計算の方法を変えたい時
                                        //3を押した後に+を押して、やっぱり×に変えたいと思ってそのまま続けて×を押した時とか
                                        if (MainWindowModel.CalCheck == true) return;

                                        // 数字が入力された上で初回に計算ボタンを押したらCalCheckをtrueにして、計算方法の仮プロパティCalに
                                        //　計算メソッドが代入されてることをPushのif文の条件分岐で使えるようにする
                                        MainWindowModel.CalCheck = true;


                                        var args = new PropertyChangedEventArgs(nameof(Result));
                                        PropertyChanged(this, args);
                                    }
                                    else
                                    {
                                        return;
                                    }
                                },
                                (param) => true
                            );

            Equal = new DelegateCommand
                        (
                            () =>
                            {
                                //=が押された後に続けてもう一回=を押しても、既にEqualCheckがtrueになってるので、ボタンクリックを無効にする
                                if (MainWindowModel.EqualCheck == true || MainWindowModel.CalCheck == true) return;
                                MainWindowModel.KeepNums.Add(model.ConvertResultDouble());
                                double sum = 0;
                                for (int i = 0; i < MainWindowModel.KeepNums.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        sum = MainWindowModel.KeepNums[i];
                                    }
                                    else
                                    {
                                        sum = MainWindowModel.Cals[i - 1](sum, MainWindowModel.KeepNums[i]);
                                    }
                                }


                                model.ConvertResult(sum);
                                Result = model.Result;


                                MainWindowModel.CalCheck = false;
                                MainWindowModel.EqualCheck = true;
                                MainWindowModel.PointCheck = false;
                                MainWindowModel.Cal = null;
                                MainWindowModel.KeepNum = 0;
                                MainWindowModel.KeepNums = null;
                                MainWindowModel.KeepNums = new List<double>();
                                MainWindowModel.Cals = null;
                                MainWindowModel.Cals = new List<Func<double, double, double>>();

                                var args = new PropertyChangedEventArgs(nameof(Result));
                                PropertyChanged(this, args);
                            },

                            () => true                              //      (model.Result != null && MainWindowModel.KeepNum != 0 && MainWindowModel.Cal != null)
                        );


            Clear = new DelegateCommand
                        (
                                () =>
                                {
                                    MainWindowModel.CalCheck = false;
                                    MainWindowModel.PointCheck = false;
                                    MainWindowModel.Cal = null;
                                    MainWindowModel.KeepNum = 0;
                                    MainWindowModel.KeepNums = null;
                                    MainWindowModel.KeepNums = new List<double>();
                                    MainWindowModel.Cals = null;
                                    MainWindowModel.Cals = new List<Func<double, double, double>>();
                                    Result = null;
                                    model.SetResultNull();
                                    var args = new PropertyChangedEventArgs(nameof(Result));
                                    PropertyChanged(this, args);
                                },
                                () => true
                        );


            AllClear = new DelegateCommand
                        (
                                () =>
                                {
                                    MainWindowModel.CalCheck = false;
                                    MainWindowModel.PointCheck = false;
                                    MainWindowModel.Cal = null;
                                    MainWindowModel.KeepNum = 0;
                                    MainWindowModel.KeepNums = null;
                                    MainWindowModel.KeepNums = new List<double>();
                                    MainWindowModel.Cals = null;
                                    MainWindowModel.Cals = new List<Func<double, double, double>>();
                                    Result = null;
                                    model.SetResultNull();
                                    var args = new PropertyChangedEventArgs(nameof(Result));
                                    PropertyChanged(this, args);
                                },
                                () => true
                        );
        }

        public double Add(double a, double b)
        {
            return a + b;
        }

        public double Sub(double a, double b)
        {
            return a - b;
        }

        public double Mul(double a, double b)
        {
            return a * b;
        }

        public double Div(double a, double b)
        {
            if (b == 0)
            {
                return a + 0;
            }
            return a / b;
        }

        public double Rem(double a, double b)
        {
            return a % b;
        }
    }
}
