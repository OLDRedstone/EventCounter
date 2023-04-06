<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RichTextLabel
	Inherits System.Windows.Forms.UserControl

	'UserControl 重写释放以清理组件列表。
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Windows 窗体设计器所必需的
	Private components As System.ComponentModel.IContainer

	'注意: 以下过程是 Windows 窗体设计器所必需的
	'可以使用 Windows 窗体设计器修改它。  
	'不要使用代码编辑器修改它。
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		SuspendLayout()
		' 
		' RichTextLabel
		' 
		AutoScaleDimensions = New SizeF(7F, 17F)
		AutoScaleMode = AutoScaleMode.Font
		Name = "RichTextLabel"
		Size = New Size(96, 56)
		ResumeLayout(False)
	End Sub

End Class
