<%@ Page Title="Tournament Examples" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Examples.aspx.cs" Inherits="RPS_Challenge.Examples" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <section class="examples-section page-card">
        <div class="module-header">
            <h3>Tournament examples</h3>
            <span class="module-subtitle">Download sample files and start a tournament</span>
        </div>
        <ol class="round">
            <asp:Repeater ID="tournamentFilesRepeater" runat="server">
                <ItemTemplate>
                    <li>
                        <h4>
                            <a href='<%# Eval("DownloadUrl") %>'><%# Eval("FileName") %></a>
                        </h4>
                        <span class='<%# Eval("DescriptionCssClass") %>'><%# Eval("Description") %></span>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ol>
    </section>
</asp:Content>
