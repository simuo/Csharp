   M S B A M L   `   `   `  ����B  >OQC_IN, Version=1.8.12.1, Culture=neutral, PublicKeyToken=null3clr-namespace:OQC_OUT.ControlsOQC_OUT.Controls  5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   CancelButtonStyle  �� 
TargetType �� Property �� Value �� 
TargetType �� Stretch �� Triggers P� Property P� Value  LoadingCircleStyle �� CenterX	 �� CenterY
 �� Triggers �� Binding �� RelativeSource �� AncestorType �� Path �� Value N� EnterActions ��Name �� TargetProperty W� From	 W� To g� Duration g� RepeatBehavior �� Target N� ExitActions �� BeginStoryboardName!    OQC_OUT.Controls.BusyChrome j� Viewport j� ViewboxUnits L� Drawing � Brush � Geometry �� Rect  � ColumnDefinitions! �� Orientation�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      (localclr-namespace:OQC_OUT.Controls  5      %Z
  &	        &	     '       7  7   �� 5      "  ��6&   .�5   
   ��  � ��
 �����6'   �� 5   
    �@�FocusVisualStyle 6-   Z� �� 5   
    �@J�SnapsToDevicePixels . 60   �� 5	   
    � �� 5
      �� 5      " ��6"   .R�5      � 
������ .+�5      �� $��12��6!   $��12��6,   ._�5      F� +����        �@B @B �    @B �@B     	$ Uniform=�6>   #��z��6P   $��2��6v   5      5       5      P� 5       �@J�IsMouseOver . . �5      ��  � �� �#aaa6;   5      P� 5       � �� . . �5      ��  � �� �#4446;   5      5      5      5      �� 5      "  ��6'   .�5   
   ��  � ��
 ���� 6'   �� 5   
    �@J�IsHitTestVisible .  6-   �� 5    
    �@J�SnapsToDevicePixels .  60   �� 5!   
    � �� 5"      �� 5#      " ��6"   .R�5$      �� $��
loadingImg��6[   $+��%/OQC_OUT;component/Images/loading.png��6   $
��None=�6L   z�5%      �� 5&      $ 25Q�6/   $	 25Q�6<   ��5(      �� 5)      
 5*   "   �� 5+   &    62   ��  �� $ local:BusyAdornerL�$ 
Visibility� Visible6�    5,   *   �� 5-   .   $ rotatingStoryboard��6>   .��5.   2   �� .��5/   6   W� $5 /(Image.RenderTransform).(RotateTransform.Angle)�51   )   $ 0X�52   )   $	 360X�62   $ 0:0:1D�6;   $ Forever��6L    50   )   �� *TemplatedParent+52   L   53   3   54   /    56   *   �� 57   .   $ rotatingStoryboard��6?   59   '   5;      5=      5>      5@      5A      �� 5B      "    6   .�5C   
   ��  �@J�Opacity$ 1Q�6$   �� 5D   
    � �� 5E      �� 5F      "   6"   .R�5G      � .+�5H      �� ��5J      L� 5K   "   $ 0,0,1,1�6/   $ Absolute=�6C    5L   &   I� 5M   *   .��5O   .   �  �#4000 5P   2   �� 5Q   6   $ 0,0,1,1�6H   5S   /   5T   +   5V   #   5X      �� 5Y      $��Center=�6!   $��Center=�6>   
�������$��0q�6p   ._�5Z      �   5[   "   �� 5\   &   $
��Auto �67   �� 5]   &   $
��Auto �67   �� 5^   &   $
��Auto �67   .+�5`   "   �� $��60��6-   $��0��69   $! 
Horizontal=�6I   $��Center=�6b   $��Center=�6   .+�5a   &   �� 8��  67   �� 5b   &   $��	PART_Text��6,   $��22(�6>   $
��Bold"�6L   $��Center=�6^   $��Center=�6{   $��0,0,4,0q�6�   6�   5c   #   �� 5d   "   $��1��6/   ���#323232{�.  $��1��6f   $y�	Collapsed=�6q   �� 5e   "   $��PART_Cancel��6)   $��60��6<   8�� $��2��6r   $y�	Collapsed=�6�   5f      5g      5h      5i      5k      5l      5m      