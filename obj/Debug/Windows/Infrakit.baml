   M S B A M L   `   `   `  ����C  ?Infrakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Infrakit.WindowsInfrakit.Windows  5          Infrakit.Windows.Infrakit   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:Infrakit.Windows     5        � Title$  Infrakit settings v.1.0.2��5      $	��555��5	      $	��520��5
       � 
ResizeMode$ NoResize=�5       � WindowStartupLocation$ CenterOwner=�5       � Icon$. (/Infrakit;component/Windows/Infrakit.ico��5      .��5      �  � ColumnDefinitions 5   
    � RowDefinitions 5   
   �� 5      $	��522 �6   .+�5   
   � -   5         �Name$ 	LogiSisse��5      $��5,8,10,6q�6"   $	��505��64   $	��510��6@    5      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   �� 5      $��* �6#    5      �� 5      $
��Auto �6    �� 5       $
��Auto �6    �� 5!      $��* �6    .+�5#      �� $��Center=�6   $��Username��63   $��2��6C   $��4,7,7,7q�6W   $��18��6h   $��55��6t   �� 5$      -   5         ��Name$ Kasutajanimi��5$      $��2��6,   $��Right=�6<   $��	0,7,164,7q�6X    �� TextWrapping$
 Wrap=�6k   $	��270��6   $��18��6�   �� 5%      $��Password��6   $��2��6(   $��4,0,7,0q�6<   $��1��6M   $��55��6Z   $��18��6e   $	��Top=�6q   �� 5&      -   5        $ Parool��5&      $��2��6&   $
��Left=�66   $��16��6Q   $��4.666,1,0,0q�6]   $
 Wrap=�6r   $	��Top=�6�   $	��225��6�   $��1��6�   $y�	Collapsed=�6�   G� 5'      -   5        	 G�Name$	 ParoolPasswordBox��5'      $��2��65   $
��Left=�6E   $��18��6`   $��5,0,0,0q�6l   $��1��6}   $	��Top=�6�   $	��224��6�   6�   �� 5(      -   5        
 ��Name$
 ShowPasswordCheckBox��5(      $��Show Password��65   $��2��6�   $��240.333,2,90.667,20q�6�   $��15��6�   $��1��6�   $	��110��6�   �� 5)      -   5         ��Name$ BtnNameLogiSisse��5)      $��Login��6/   $
��Left=�6?   $��360,30,0,18q�6Z   $��62��6o    � ToolTip$, &Projects are received from the server.��6�    �� 	IsDefault . $��2��6�   $��2��6�   �� 5+      -   5        $( "BtnNameSalvestaSisselogimineAndmed��5+      $��Save login information��6A   $
��Left=�6b   $��20��6}   $��	289,7,0,0q�6�   $	��Top=�6�   $	��133��6�   $��2��6�   $H BLogin information is saved to computer registry in encrypted form.��6�   �� 5,      -   5         ��Name$ 	Projektid��5,      $
��Left=�6*   $��20��6E   $��5,24,0,0q�6Q   $	��Top=�6c   $	��418��6{   $��1��6�   $��2��6�   $��2��6�   �� 5.      $��Surfaces folder��6   $��3��6/   $��4,15,247,400q�6C   $��2��6Y   �� 5/      -	   5        $ PindadeKaustad��5/      $��3��6/   $
��Left=�6C   $��20��6^   $��4,39,0,0q�6j   $	��Top=�6|   $	��235��6�   $��2��6�   R� 50      -
   5         R�Name$ SurfaceTreeView��50      $��3��60   $��4,125,266,131q�6D   $��2��6[   $	��185��6h   ��51      �� 52       �� 
TargetType" Q�6   .�53      ��  �� Property � �� �� Value 54      �� 55   "    �� 
TargetType" Q�62   .R�56   &   ��  �� Orientation$ 
Horizontal=�61   .+�57   *   �� -   5        $��Center=�58   -    d� 	IsChecked 57   3   �� *
IsSelected+ �� Mode$ TwoWay=�58   -   �� 59   *   ��6;   �� *Name+5:   '   5;   #   5=      5>      5@      �� 5A      -   5        $ 
OtsiPinnad��5A      $��3��6*   $
��Left=�6>   $��20��6Y   $��4,99,0,0q�6e   $��2��6w   $
 Wrap=�6�   $	��Top=�6�   $	��235��6�   6�   �� 5B      -   5        $ BtnValiPinnad��5B      $��Select surfaces��6,   $
��Left=�6F   $��25��6a   $��4,70,0,0q�6m   $��2��6   $	��Top=�6�   $	��115��6�   $" Select surfaces from drawing��6�    J� RenderTransformOrigin$ 
-9.29,0.23&�6�   $��3��6  �� 5C      -   5        $
 ValitudPinnadNähtavad��5C      $��Hide unselected��66   $��2��6P   $
��Left=�6`   $��20��6{   $��	65,75,0,0q�6�   $��2��6�   $	��Top=�6�   $	��115��6�   $ -7.929,-1.963&�6  �� 5E      $��Alignments folder��6   $��0,15,-3,401q�61   $��2��6F   $��2��6V   $��25��6c   $��Right=�6o   $	��254��6�   �� 5F      -   5        $ TelgedeKaustad��5F      $
��Left=�6/   $��20��6J   $��
187,39,0,0q�6V   $	��Top=�6j   $	��235��6�   $��2��6�   $��2��6�   R� 5G      -   5        $ AlignmentTreeView��5G      $��188,126,17,130q�62   $��2��6J   $��2��6W   $	��185��6g   ��5H      �� 5I       �� ItemsSource 6/   �� *
Profiles+.R�5J      �� -   5        ��5J   #   �� *Name+ 6<   �� *
IsSelected+$ TwoWay=�6�    �� ItemTemplate 5K      �� 5L      .R�5M   "   �� -   5        ��5M   +   �� *Name+ 6D   �� *IsSubItemSelected+$ TwoWay=�{�5N   '   ��  �� RelativeSource ��  �� AncestorType" Q� �� Path$ DataContext.IsSelected�5O      5Q      5S      �� 5T      -   5        $ 
OtsiTeljed��5T      $
��Left=�6*   $��20��6E   $��188,100,0,0q�6Q   $��2��6f   $
 Wrap=�6s   $	��Top=�6�   $	��235��6�   $��2��6�   �� 5U      -   5        $ BtnValiTeljed��5U      $��Select alignments��6,   $��2��6H   $��Right=�6X   $��25��6t   $��
0,70,132,0q�6�   $��2��6�   $	��Top=�6�   $	��119��6�   $$ Select alignments from drawing��6�   �� 5V      -   5        $
 ValitudTeljedNähtavad��5V      $��Hide unselected��66   $��2��6P   $
��Left=�6`   $��20��6{   $��
315,75,0,0q�6�   $��2��6�   $	��Top=�6�   $	��115��6�   $ 19.905,0.148&�6   �� 5X      $��Folder to save surfaces��6   $��3��67   $��4,316,361,99q�6K   $��2��6a   �� 5Y      -   5        $ BtnNameValiPinnadKaust��5Y      $��Browse folder��65   $
��Left=�6M   $��20��6h   $��
78,315,0,0q�6t   $	��Top=�6�   $��95��6�   $ Button to select a folder��6�   $��2��6�   $��2��6�   �� 5Z      -   5        $ 
PinnaKaust��5Z      $
��Left=�6*   $��20��6E   $��	4,340,0,0q�6Q   $
 Wrap=�6d   $	��Top=�6x   $	��235��6�   $H BFolder where surface xml are saved. When empty then to DWG folder.��6�   $��2��6  $��3��6  �� 5\      -   5        $ BtnNameLaePinnadUlesseUus��5\      $��Upload surfaces��68   $��25��6R   $��4,370.333,390.667,0q�6^   $	��Top=�6{   $^ XSurfaces xml files are saved to surfaces folder and uploaded to selected project folder.��6�   $��2��6  $��3��6!  $	��105��65  �� 5^      $��Folder to save aligmnets��6   $��188,314,108,99q�68   $��2��6P   $��2��6`   �� 5_      -   5        $ BtnNameValiTeljeKaust��5_      $��Browse folder��64   $��2��6L   $
��Left=�6\   $��20��6w   $��327,315,0,0q�6�   $	��Top=�6�   $��95��6�   $ Button to select a folder��6�   $��2��6�   $ 0.229,0.063&�6  �� 5`      -   5        $ 
TeljeKaust��5`      $
��Left=�6*   $��20��6E   $��187,340,0,0q�6Q   $
 Wrap=�6f   $	��Top=�6z   $	��235��6�   $H BFolder where surface xml are saved. When empty then to DWG folder.��6�   $��2��6  $��2��6   �� 5a      -   5        $ BtnNameLaeTeljedUlesseUus��5a      $��Upload alignments��68   $��25��6T   $��307,370,17,0q�6`   $	��Top=�6v   $b \Alingments xml files are saved to alignments folder and uploaded to selected project folder.��6�   $��2��6  $��2��6   $ 0.854,0.467&�60  �� 5b      -   5        $ BtnNameLaeUlesseUus��5b      $��
Upload all��62   $��25��6G   $��130,370,204,0q�6S   $	��Top=�6j   $e _Surface and alingments xml files are saved to folders and uploaded to selected project folders.��6�   $��2��6  $ 
0.457,4.24&�6  $��2��64  �� 5c      $��Projects��6   $��2��6(   $��4,26,4.541,0q�6<   $��1��6R   $��55��6_   $��18��6j   $	��Top=�6v   $��2��6�   5e      �� 5f   
   $ 
Horizontal=�6   $��Right=�6.   $��0,475,15,10q�6J   $	��168��6_   .+�5g      �� -   5        $ btnOk��5g   >   $��75��6   $��Ok��6     . $��4q�6M   $��25��6l   �� 5h      -   5        $ 	btnCancel��5h   B   $��75��6   $��Cancel��6     �� IsCancel .  $��4q�6U   $��25��6x   5i      5j      5k      