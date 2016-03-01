<%@ Page Language="C#" AutoEventWireup="true"  CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication1.WebForm1" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            height: 24px;
            width: 432px;
        }
        .auto-style3 {
            height: 24px;
            width: 525px;
        }
        .auto-style4 {
            width: 525px;
        }
        .auto-style5 {
            width: 432px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:TextBox ID="TextBox1" runat="server" Height="314px" MaxLength="2147483647" TextMode="MultiLine" Width="586px"></asp:TextBox>
    
        <asp:TextBox ID="TextBox3" runat="server" Height="314px" MaxLength="2147483647" TextMode="MultiLine" Width="586px"></asp:TextBox>
        <asp:Button ID="Button5" runat="server" OnClick="Button5_Click" Text="開始抓商品資料" />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="第一次過濾" />
        <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>
        <table class="auto-style1">
            <tr>
                <td class="auto-style3">
                    <asp:Label ID="StartTimeLabel" runat="server"></asp:Label>
                </td>
                <td class="auto-style2">
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                    </asp:ScriptManager>
                    <asp:Label ID="SpanLabel" runat="server"></asp:Label>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick">
                            </asp:Timer>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="auto-style4">
        <asp:Label ID="StatuLabel" runat="server"></asp:Label>
    
                </td>
                <td class="auto-style5">
                    <asp:Label ID="ProductINFOLabel" runat="server"></asp:Label>
                </td>
                <td></td>
            </tr>
        </table>
    
    </div>
        <asp:TextBox ID="TextBox2" runat="server" Height="241px" MaxLength="2147483647" TextMode="MultiLine" Width="585px"></asp:TextBox>
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="變成CSV" />
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="商品資料更新" />
        <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="商品爬蟲" />
    </form>
</body>
</html>
