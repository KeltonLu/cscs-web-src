
//document.oncontextmenu=new Function("event.returnValue=false;");  //禁止右键功能,单击右键将无反应

document.onkeydown=function() 
{
    //alert(event.keyCode);
    //禁用回車
    if (event.keyCode==13 && event.srcElement.type != "textarea")
    {
        return false; 
    }
    //禁用Shift
    //if (event.keyCode==16)
    //{
    //    return false; 
    //}
    //禁用Shift
    //if(event.shiftKey)
    //{ 
    //   return false;
    //}
    
    //禁用刷新，回退
    //if ( (event.altKey) || ((event.keyCode == 8) && (event.srcElement.type != "text" && event.srcElement.type != "textarea" && 
    //    event.srcElement.type != "password")) || 
    //    ((event.ctrlKey) && ((event.keyCode == 78) || (event.keyCode == 82)) ) || 
    //    (event.keyCode == 116) ) 
    //    { 
    //    event.keyCode = 0; 
    //    event.returnValue = false; 
    //    } 
}

/**
 * 加上逗號
 *
 * @param num 
 * return 正確數字
 */
function GetValue(num)
{
    if(num=="")
        return "";
        
    num=Number(num);
    
    if(isNaN(num))  
        return 0;
            
    num=num+"";
    
    var arrayNum = num.split('.');
      
    var returnNum=arrayNum[0];
      
    var re=/(-?\d+)(\d{3})/;
    while(re.test(returnNum))
    {       
        returnNum=returnNum.replace(re,"$1,$2")       
    }
    if(arrayNum.length==2)
    {   
        returnNum+="."+arrayNum[1];  
    }
    
    return returnNum;
}   


/**
 * 去掉逗號
 *
 * @param obj 
 */
function DelDouhao(obj)
{
        obj.value=obj.value.replace(/,/g,'');
        obj.select();
}

/**
 * 驗證輸入數字（整數檢查，可以為負整數）
 *
 * @param TextName
 * @param NumLen
 * return 正確數字
 */
function CheckNumWithNoId1(obj,NumLen)
{
    var CardNum = obj.value;
                
    var patten=new RegExp(/^-?\d*$/); 
    var IsCal=false;
    
    if(!patten.test(CardNum))
    {
        IsCal=true;
    }
        
    var CardNum = CardNum.replace(/[^0-9]/g,'');
    
    if(CardNum.length>NumLen)
    {
        IsCal=true;
        CardNum=CardNum.substring(0,NumLen);
    }
    
    if(IsCal)
    {
        obj.value=CardNum;
    }
    
    return CardNum;
}

/**
 * 驗證輸入數字
 *
 * @param TextName
 * @param NumLen
 * return 正確數字
 */
function CheckNumWithNoId(obj,NumLen)
{
    var CardNum = obj.value;
                
    var patten=new RegExp(/^\d*$/); 
    var IsCal=false;
    
    if(!patten.test(CardNum))
    {
        IsCal=true;
    }
        
    var CardNum = CardNum.replace(/[^0-9]/g,'');
    
    if(CardNum.length>NumLen)
    {
        IsCal=true;
        CardNum=CardNum.substring(0,NumLen);
    }
    
    if(IsCal)
    {
        obj.value=CardNum;
    }
    
    return CardNum;
}

/**
 * 驗證輸入數字
 *
 * @param TextName
 * @param NumLen
 * return 正確數字
 */
function CheckAmtWithNoId(obj,NumLen,DecLen)
{
   var CardAmt = obj.value;
    
    if(CardAmt==".")
    {
        obj.value="";
        return;
    }
        
                
    var IsCal=false;
        
    var patten = new RegExp(/(^[1-9]\d*([.])?(\d{1,2})?$)|(^\d*$)|(^[0][.](\d{1,2})?$)/);

    if(!patten.test(CardAmt))
    {
       IsCal=true;
    }
        
    CardAmt = CardAmt.replace(/[^0-9.]/g,'');
    
    var arrayAmt = CardAmt.split('.');
    if(arrayAmt[0]!=""&&arrayAmt[0]!=undefined)
    {
        if(arrayAmt[0].length>NumLen)
        {
            IsCal=true;
            arrayAmt[0]=arrayAmt[0].substring(0,NumLen);
        }
         CardAmt=arrayAmt[0];
    }
   
    if(arrayAmt.length==2)
    {
        if(arrayAmt[1]==""||arrayAmt[1]==undefined)
        {    
            
        }
        else
        {
            
            if(arrayAmt[1].length>DecLen)
            {
                 IsCal=true;
                 arrayAmt[1]=arrayAmt[1].substring(0,DecLen);
            }
        }  
        CardAmt+="."+arrayAmt[1];  
    }
    
    if(IsCal)
        obj.value=CardAmt;
    
    return CardAmt;   
}


/**
 * 驗證輸入浮點
 *
 * @param TextName 
 * @param NumLen
 * @param DecLen
 * return 正確數字
 */
function CheckAmt(TextName,NumLen,DecLen)
{
    var CardAmt = document.getElementById(TextName).value;
    
    if(CardAmt==".")
    {
        document.getElementById(TextName).value="";
        return;
    }
        
                
    var IsCal=false;
        
    var patten = new RegExp(/(^[1-9]\d*([.])?(\d{1,2})?$)|(^\d*$)|(^[0][.](\d{1,2})?$)/);

    if(!patten.test(CardAmt))
    {
       IsCal=true;
    }
        
    CardAmt = CardAmt.replace(/[^0-9.]/g,'');
    
    var arrayAmt = CardAmt.split('.');
    if(arrayAmt[0]!=""&&arrayAmt[0]!=undefined)
    {
        if(arrayAmt[0].length>NumLen)
        {
            IsCal=true;
            arrayAmt[0]=arrayAmt[0].substring(0,NumLen);
        }
         CardAmt=arrayAmt[0];
    }
   
    if(arrayAmt.length==2)
    {
        if(arrayAmt[1]==""||arrayAmt[1]==undefined)
        {    
            
        }
        else
        {
            
            if(arrayAmt[1].length>DecLen)
            {
                 IsCal=true;
                 arrayAmt[1]=arrayAmt[1].substring(0,DecLen);
            }
        }  
        CardAmt+="."+arrayAmt[1];  
    }
    
    if(IsCal)
        document.getElementById(TextName).value=CardAmt;
    
    return CardAmt;    
}


/**
 * 驗證輸入數字
 *
 * @param TextName
 * @param NumLen
 * return 正確數字
 */
function CheckNum(TextName,NumLen)
{
    var CardNum = document.getElementById(TextName).value;
                
    var patten=new RegExp(/^\d*$/); 
    var IsCal=false;
    
    if(!patten.test(CardNum))
    {
        IsCal=true;
    }
        
    var CardNum = CardNum.replace(/[^0-9]/g,'');
    
    if(CardNum.length>NumLen)
    {
        IsCal=true;
        CardNum=CardNum.substring(0,NumLen);
    }
    
    if(IsCal)
    {
        document.getElementById(TextName).value=CardNum;
    }
    
    return CardNum;
}
/**
 * 驗證輸入數字,
 *
 * @param TextName
 * @param NumLen
 * return 正確數字
 */
function CheckIntNum(TextName,NumLen)
{
    var CardNum = document.getElementById(TextName).value;
                
    var patten=new RegExp(/^-?\d*$/); 
    var IsCal=false;
    
    if(!patten.test(CardNum))
    {
        IsCal=true;
    }
        
    var CardNum = CardNum.replace(/[^0-9]/g,'');
    
    if(CardNum.length>NumLen)
    {
        IsCal=true;
        CardNum=CardNum.substring(0,NumLen);
    }
    
    if(IsCal)
    {
        document.getElementById(TextName).value=CardNum;
    }
    
    return CardNum;
}

/**
 * 取字符串長度
 *
 * @param sString
 * return 長度
 */
function getLen(sString)
{
    var sStr,iCount,i,strTemp ; 
    iCount = 0 ;
    sStr = sString.split("");
    for (i = 0 ; i < sStr.length ; i ++)
    {
         strTemp = escape(sStr[i]);
         if (strTemp.indexOf("%u",0) == -1)
         {
              iCount = iCount + 1 ;
         }
         else
         {
              iCount = iCount + 2 ;
         }
     }
 
     return iCount ;
} 

/**
 * 驗證輸入框長度，超過自動截掉
 *
 * @param obj　控件
 * @param 長度
 */
function LimitLengthCheck(obj, byteLength)
{
	var len = getLen(obj.value);
	
	if(len <= byteLength)
	{
		return true;
	}
	else
	{
        var str = obj.value;
	    while(getLen(str)>byteLength)
        {
	        str = str.substr(0,str.length-1);
        }
        
        obj.value = str;
        obj.focus();   
    }
}

function CheckClientValidate()
{
   return Page_ClientValidate();
   //if (Page_IsValid)
   //{
   //    return true;
   //}
   //else
  // {
   //    return false;
  // }
}


/**
 * 頁面回退
 *
 * @param Url　回退地址
 */
function returnform(Url)
{
    window.location=Url+"?Con=1";
} 
/*
檢查日期格式：yyyy/MM/dd
*/
function isDate(dateStr) {
    var matchArray = dateStr.match(/^[0-9]+\/[0-1][0-9]\/[0-3][0-9]$/)
    if (matchArray == null) {
      alert("請輸入正確的日期格式！");
      return false;
    }
    return true;
} 