   M S B A M L   `   `   `  ����B  >SQL2XLS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:SQL2XLSSQL2XLS  5   	       SQL2XLS.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:SQL2XLS     5   	     � Title$  
MainWindow��5   	   $	��554��6   $	��628��6)   .��5	      � .+�5
   
   �� -   5         ��Name$ 
InputPanel��5
      $��sql-Datei ablegen��6$   $��10,10,245,0q�6@   $��1,1,1,1q�6U   
����������	#FFD1D1D1$	��120��6�   $	��Top=�6�   $��Center=�6�   $��Center=�6�   $
��Bold"�6  $��Bahnschrift Light)�6  $��18(�65  ���	#FF868686 J� 	AllowDrop . 6�  �� 5   
   -   5         ��Name$ ExcelExport��5      $��.xls-Export��6&   $��46��6<   $��	0,10,10,0q�6H   $	��Top=�6[   $��Right=�6s   $	��215��6�   ��5      �� 5       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5      �  � Color$ 	#FFA4FF98��6#   � 5      $ 	#FF37D221��6#    � Offset$ 1Q�65   5      5      �� 5   
   -   5        $ 
TextExport��5      $��Textbox exportieren��6%   $��46��6C   $��	0,84,10,0q�6O   $	��Top=�6b   $��Right=�6z   $	��215��6�   ��5      �� 5      $ 0.5,1&�6&   $ 0.5,0&�67   .��5      � $ 	#FFE3F2FF��6#   � 5      $ 	#FFAEC3D6��6#   $ 1Q�65   5      5      �� 5   
   -   5         ��Name$ 	OutputBox��5      $��10,150,10,10q�6)   ���	#FFF7F7F7���	#FFAFAFAF$��Consolas)�6n   �� 5   
   -   5        	 ��Name$	 Arrow��5      $��24��6#   $��0,126,102,0q�6/   $
��Wrap=�6D   $��5��6X   $	��Top=�6a   
 J� RenderTransformOrigin$
 0.5,0.5&�6y   $��Webdings)�6�   $��36(�6�   $��Right=�6�   $��32��6�   5      5      