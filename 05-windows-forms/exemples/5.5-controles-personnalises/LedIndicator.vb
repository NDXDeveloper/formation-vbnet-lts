' ============================================================================
'  Section 5.5 : Contrôles personnalisés et UserControl
'  Description : Le contrôle DESSINÉ de la section : on hérite de Control et
'                l'on redéfinit OnPaint (GDI+). SetStyle active un rendu sans
'                scintillement, Invalidate() déclenche un redessin, et
'                SystemColors s'adapte au mode clair/sombre (section 5.2).
'  Fichier source : 05-controles-personnalises.md
' ============================================================================

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

Public Class LedIndicator
    Inherits Control

    Private _allume As Boolean

    Public Sub New()
        ' Dessin optimisé, sans scintillement, redessiné au redimensionnement
        SetStyle(ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw, True)
    End Sub

    <Category("Comportement")>
    <DefaultValue(False)>
    Public Property Allume As Boolean
        Get
            Return _allume
        End Get
        Set(value As Boolean)
            If _allume <> value Then
                _allume = value
                Invalidate()   ' déclenche un nouveau OnPaint
            End If
        End Set
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' SystemColors s'adapte au mode clair/sombre (section 5.2)
        Dim couleur As Color = If(_allume, Color.LimeGreen, SystemColors.ControlDark)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Using pinceau As New SolidBrush(couleur)
            e.Graphics.FillEllipse(pinceau, 2, 2, Width - 4, Height - 4)
        End Using
    End Sub

End Class
