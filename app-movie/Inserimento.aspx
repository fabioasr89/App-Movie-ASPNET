<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inserimento.aspx.cs" Inherits="app_movie.Inserimento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="margin-top:5%;padding:5%;background-color:aliceblue;">
            <div class="row">
                <div class="col-6" style="margin-bottom:1%;">
                    <asp:TextBox runat="server" placeholder="Titolo" ID="Titolo" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-6" style="margin-bottom:1%;">
                     <asp:DropDownList ID="Anni" runat="server" CssClass="form-control" OnLoad="Anni_Load">
                     </asp:DropDownList>
                </div>
                <div class="col-6" style="margin-bottom:1%;">
                    <asp:DropDownList ID="Generi" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <div class="col-6" style="margin-bottom:1%;">
                    <asp:FileUpload runat="server" ID="Copertina" style="float" />
                </div>
                    <div class="col-6">
                        <asp:Button CssClass="btn btn-primary" runat="server" Text="Inserisci" style="float:left;" ID="Inserisci" OnClick="Inserisci_Click"/>
                    </div>
                    <div class="col-6">
                       <asp:Button CssClass="btn btn-primary" runat="server" Text="Reset"  ID="Reset" OnClick="Reset_Click"/>
                    </div>
            </div>

    
    </div>
   
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>

</asp:content>
