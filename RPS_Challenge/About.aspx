<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="RPS_Challenge.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <section class="page-card">
        <h2>About</h2>
        <h3>About this project</h3>
        <p>
            This solution resolves Rock-Paper-Scissors tournaments, including multi-round bracket validation based on
            tournament definition files.
        </p>
        <p>
            In this challenge I used ASP.NET, Entity Framework, and LINQ to build tournament processing logic,
            update score data, and keep the application structure simple and clear.
        </p>
        <p>
            The application has been modernized in UI/UX to improve readability and user experience while preserving
            the original challenge intent.
        </p>

        <h3>About the author</h3>
        <p>
            I am Victor Viquez Benavides, a Systems Engineer graduated from Universidad Latina of Costa Rica in 2006.
        </p>
        <p>
            I have experience in developing web applications based on Microsoft technologies such as ASP.NET and C#.
        </p>
        <p>
            I chose these technologies because I have worked with ASP.NET for many years, and this challenge
            was also a way to continue learning and improving with newer practices.
        </p>
    </section>
</asp:Content>
