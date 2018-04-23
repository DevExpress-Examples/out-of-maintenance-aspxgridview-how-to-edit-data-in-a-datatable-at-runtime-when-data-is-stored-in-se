Imports System
Imports System.Data
Imports DevExpress.Web.ASPxGridView
Imports DevExpress.Web.Data


Partial Public Class _Default
    Inherits System.Web.UI.Page

    Public Property CustomDataSource() As DataTable
        Get
            If Session("CustomTable") Is Nothing Then
                Session("CustomTable") = CreateDataSource()
            End If
            Return DirectCast(Session("CustomTable"), DataTable)
        End Get
        Set(ByVal value As DataTable)
            Session("CustomTable") = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        CreateGrid()
    End Sub

    Protected Sub grid_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        TryCast(sender, ASPxGridView).DataSource = CustomDataSource
    End Sub

    Protected Sub grid_RowDeleting(ByVal sender As Object, ByVal e As ASPxDataDeletingEventArgs)

        Dim id_Renamed As Integer = CInt((e.Keys(0)))
        Dim dr As DataRow = CustomDataSource.Rows.Find(id_Renamed)
        CustomDataSource.Rows.Remove(dr)

        Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
        grid.CancelEdit()
        e.Cancel = True
    End Sub
    Protected Sub grid_RowUpdating(ByVal sender As Object, ByVal e As ASPxDataUpdatingEventArgs)

        Dim id_Renamed As Integer = CInt((e.OldValues("Id")))
        Dim dr As DataRow = CustomDataSource.Rows.Find(id_Renamed)
        dr(0) = e.NewValues("Id")
        dr(1) = e.NewValues("Data")

        Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
        grid.CancelEdit()
        e.Cancel = True
    End Sub
    Protected Sub grid_RowInserting(ByVal sender As Object, ByVal e As ASPxDataInsertingEventArgs)
        CustomDataSource.Rows.Add(e.NewValues("Id"), e.NewValues("Data"))

        Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
        grid.CancelEdit()
        e.Cancel = True
    End Sub

    Protected Sub grid_InitNewRow(ByVal sender As Object, ByVal e As ASPxDataInitNewRowEventArgs)
        Dim grid As New ASPxGridView()
        e.NewValues("Id") = grid.GetHashCode()
    End Sub

    Private Sub CreateGrid()
        Dim grid As New ASPxGridView()
        grid.ID = "grid"
        Form.Controls.Add(grid)
        grid.KeyFieldName = "Id"

        AddHandler grid.DataBinding, AddressOf grid_DataBinding
        AddHandler grid.RowDeleting, AddressOf grid_RowDeleting
        AddHandler grid.RowUpdating, AddressOf grid_RowUpdating
        AddHandler grid.RowInserting, AddressOf grid_RowInserting
        AddHandler grid.InitNewRow, AddressOf grid_InitNewRow
        grid.DataBind()

        If Not IsPostBack Then
            Dim c As New GridViewCommandColumn()
            grid.Columns.Add(c)
            c.ShowEditButton = True
            c.ShowNewButton = True
            c.ShowDeleteButton = True
        End If
    End Sub

    Private Function CreateDataSource() As DataTable
        Dim dataTable As New DataTable("DataTable")
        dataTable.Columns.Add("Id", GetType(Int32))
        dataTable.PrimaryKey = New DataColumn() { dataTable.Columns(0) }
        dataTable.Columns.Add("Data", GetType(String))
        For i As Integer = 0 To 4
            dataTable.Rows.Add(i, "Data" & i.ToString())
        Next i
        Return dataTable
    End Function
End Class