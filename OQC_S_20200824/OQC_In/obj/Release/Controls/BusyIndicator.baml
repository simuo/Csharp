   M S B A M L   `   `   `  ���� B  >OQC_IN, Version=1.8.12.1, Culture=neutral, PublicKeyToken=null3clr-namespace:OQC_OUT.ControlsOQC_OUT.Controls  W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   CancelButtonStyle  �� 
TargetType �� Property �� Value �� 
TargetType �� Stretch �� Triggers P� Property P� Value  LoadingCircleStyle �� CenterX	 �� CenterY
 �� Triggers �� Binding �� RelativeSource �� AncestorType �� Path �� Value N� EnterActions ��Name �� TargetProperty W� From	 W� To g� Duration g� RepeatBehavior �� Target N� ExitActions �� BeginStoryboardName!    OQC_OUT.Controls.BusyChrome j� Viewport j� ViewboxUnits L� Drawing � Brush � Geometry �� Rect  � ColumnDefinitions! �� Orientation�� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     (localclr-namespace:OQC_OUT.Controls  %�  &	        &	 }    '   �    7  7   �� "  ��.���  � ��
 �������  �@�FocusVisualStyle Z� ��  �@J�SnapsToDevicePixels . ��  � �� �� " ��.R�� 
������ .+��� $��12��$��12��._�F� +����        �@B @B �    @B �@B     	$ Uniform=�#��z��$��2�� P�  �@J�IsMouseOver . . ���  � �� �#aaaP�  � �� . . ���  � �� �#444�� "  ��.���  � ��
 ���� ��  �@J�IsHitTestVisible .  ��  �@J�SnapsToDevicePixels .  ��  � �� �� " ��.R��� $��
loadingImg��$+��%/OQC_OUT;component/Images/loading.png��$
��None=�z��� $ 25Q�$	 25Q����� 
 ��  ��  �� $ local:BusyAdornerL�$ 
Visibility� Visible �� $ rotatingStoryboard��.���� .��W� $5 /(Image.RenderTransform).(RotateTransform.Angle)�$ 0X�$	 360X�$ 0:0:1D�$ Forever�� �� *TemplatedParent+ �� $ rotatingStoryboard���� "    .���  �@J�Opacity$ 1Q���  � �� �� "   .R�� .+��� ��L� $ 0,0,1,1�$ Absolute=� I� .���  �#4000 �� $ 0,0,1,1��� $��Center=�$��Center=�
�������$��0q�._��   �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��60��$��0��$! 
Horizontal=�$��Center=�$��Center=�.+��� 8��  �� $��	PART_Text��$��22(�$
��Bold"�$��Center=�$��Center=�$��0,0,4,0q��� $��1�����#323232{�.  $��1��$y�	Collapsed=��� $��PART_Cancel��$��60��8�� $��2��$y�	Collapsed=�