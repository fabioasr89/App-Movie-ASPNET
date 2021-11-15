<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="app_movie._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container" style="background-color:#f8f9fa; padding:2%; width:150%;margin-top: 1%;">
        <div class="row" style="margin-top:1%;">
                <div class="col" id="filtri">
                    <asp:DropDownList ID="Generi" runat="server"  SelectionMode="single" CssClass="form-control" OnSelectedIndexChanged="Generi_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col" id="ricerca">
                   <nav class="navbar navbar-light bg-light" style="background-color:white;">
            
                       <asp:TextBox ID="Titolo"  runat="server" CssClass="form-control mr-sm-2" placeholder="Ricerca Titolo"></asp:TextBox>
                       <asp:Button CssClass="btn btn-outline-success my-2 my-sm-0" runat="server" Text="Ricerca" ID="Ricerca" OnClick="Ricerca_Click"></asp:Button>
                    </nav>
                </div>
            </div>

        <asp:GridView ID="GridView1" runat="server"  OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnPreRender="GridView1_PreRender" OnRowDataBound="GridView1_RowDataBound" >
           
         


        </asp:GridView>
    </div>

    

   

<asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>

 <script>

     $(document).ready(function () {
         function aggiungiClasseHeader() {
             $('#MainContent_GridView1>thead').addClass('thead-dark');
         }

         aggiungiClasseHeader();

     });

 </script>
   

</asp:Content>


