using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Depository013Model 的摘要描述
/// </summary>
public class Depository013Model
{
    public Depository013Model()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }
    #region Model
    private string _type;
    private string _affinity;
    private string _photo;
    private int _CardType_RID;
    private int _Factory_Perso_RID;
    private int _Stock_Number;
    private int _Stock_Number_1 ;
    private int _New_Number_B ;
    private int _New_Number_C ;
    private int _GB_Number_D1 ;
    private int _HB_Number_D2 ;
    private int _Change_Num_E ;
    private int _Income_Num_F ;
    private int _Forecast_Stock_Num_G ;
    private int _Forecast_Stock_Num_G1;
    private int _TotalXH;
    private decimal _H ;
    private int _J;
    private int _K;
    private decimal _L;
    private int _H1;
    private int _L1;

    public int TotalXH {
        get { return _TotalXH; }
        set { _TotalXH = value; }
    }
    public decimal H {
        get { return _H; }
        set { _H = value; }
    }

    public int J {
        get { return _J; }
        set { _J = value; }
    }

    public int K {
        get { return _K; }
        set { _K = value; }
    }

    public decimal L {
        get { return _L; }
        set { _L = value; }
    }

    public int H1 {
        get { return _H1; }
        set { _H1 = value; }
    }

    public int L1 {
        get { return _L1; }
        set { _L1 = value; }
    }        

    public string type {
        get { return _type; }
        set { _type = value; }
    }

    public string affinity {
        get { return _affinity; }
        set { _affinity = value; }
    }

    public string photo {
        get { return _photo; }
        set { _photo = value; }
    }

    public int CardType_RID {
        get { return _CardType_RID; }
        set { _CardType_RID = value; }
    }

    public int Factory_Perso_RID {
        get { return _Factory_Perso_RID; }
        set { _Factory_Perso_RID = value; }
    
    }

    public int Stock_Number {
        get { return _Stock_Number; }
        set { _Stock_Number = value; }
    }

    public int Stock_Number_1 {
        get { return _Stock_Number_1; }
        set { _Stock_Number_1 = value; }
    }

    public int New_Number_B {
        get { return _New_Number_B; }
        set { _New_Number_B = value; }
    }
    public int New_Number_C {
        get { return _New_Number_C; }
        set { _New_Number_C = value; }
    }

    public int GB_Number_D1 {
        get { return _GB_Number_D1; }
        set { _GB_Number_D1 = value; }
    }

    public int HB_Number_D2 {
        get { return _HB_Number_D2; }
        set { _HB_Number_D2 = value; }
    }

    public int Change_Num_E {
        get { return _Change_Num_E; }
        set { _Change_Num_E = value; }
    }

    public int Income_Num_F {
        get { return _Income_Num_F; }
        set { _Income_Num_F = value; }
    }

    public int Forecast_Stock_Num_G {
        get { return _Forecast_Stock_Num_G; }
        set { _Forecast_Stock_Num_G = value; }
    }

    public int Forecast_Stock_Num_G1 {
        get { return _Forecast_Stock_Num_G1; }
        set { _Forecast_Stock_Num_G1 = value; }
    }
    #endregion
}
