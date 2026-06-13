' ============================================================================
'  Section 6.4 : Liaison de données
'  Description : Le convertisseur de la section (IValueConverter) : un booléen
'                EstActif devient une couleur (vert / rouge). Convert
'                (source -> cible) et ConvertBack (cible -> source, ici non
'                supporté). Déclaré comme ressource et référencé par
'                Converter={StaticResource ...}.
'  Fichier source : 04-data-binding.md
' ============================================================================

Imports System.Globalization
Imports System.Windows.Data
Imports System.Windows.Media

Public Class BoolEnCouleurConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type,
                            parameter As Object, culture As CultureInfo) As Object _
                            Implements IValueConverter.Convert
        Return If(CBool(value), Brushes.Green, Brushes.Red)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type,
                                parameter As Object, culture As CultureInfo) As Object _
                                Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function
End Class
