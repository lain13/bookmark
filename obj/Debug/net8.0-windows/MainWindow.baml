   M S B A M L   `   `   `  ����C  ?Bookmark, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:BookmarkBookmark  5   	       Bookmark.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Bookmark     5   	     � Title$  북마크 관리자��5   	   $	��600��6   $	��800��6&    � 	Resources 5	         IconButtonStyle �� 
TargetType �� Property �� Value  FocusModeButtonStyle �� BasedOn �� Triggers �� Binding �� Value��%h  &	        &	 �     7   �� 5
   
   " ��6(   .�5      ��  � ��
 ���� 6+   �� 5       � ��$ 0q�60   �� 5       � ��$ 5q�6(   �� 5       � ��$ 2q�6'   5      �� 5   
   " ��6-   8    5      �� 5       6   �� *	IsFocused+ True6<   .a�5      ��  � ��
 ��ح�63   5      5      5      .��5      � 	 � RowDefinitions	 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� -   5        $��북마크 추가��5      $	��100��6<   $
��Left=�6H   $��10q�6c   �� 5    
   -   5        
 ��Name$
 BookmarkListBox��5       $��1��6+   $��10q�68    J� 	AllowDrop$
 True��5"      ��5!      �� *	Bookmarks+��5&      �� 5'      .R�5(      � 5)      ��5+      �� 5,      .R�5-      �� 
�������$��1q�60   $��5q�6D    �� Padding$ 5q�6O   ._�5.      � $	��200��6   .+�5/      �� .+�50   "   �� $��0,0,0,10q�6,   �� 51   &   -   5        ��51   0   �� *Url+.�6x   .�52   *   �� $
��Wrap=�6K   $��16(�6_   $
��Bold"�6m   ��64   �� *Title+6m   53   '   54   #   �� 55   "   $	��180��6G   $	��180��6S   ��6(   �� *QrCodeImage+ J� Opacity 6`   �� *	Opacity+56      �� 57       �� Orientation$ 
Horizontal=�6)   $��Right=�6B   $	��Top=�6^   .+�58   "   �� -   5        #��[  58   )   .��59   &   �� $��✏️��60   $��16(�6:   5:   #   �� 5;   "   -   5        #��[  5;   )   .��5<   &   �� $��🗑️��60   $��16(�6;   5=   #   �� 5>   "   -   5        #��[ 5>   )   .��5?   &   �� $��👁️��60   $��16(�6;   5@   #   5A      5B      5C      5D      5F      5G      5H      