<%

'/**
' * ASP Stats Generator - Powerful and reliable ASP website counter
' *
' * This file is part of the ASP Stats Generator package.
' * (c) 2003-2008 Simone Carletti <weppos@weppos.net>, All Rights Reserved
' *
' * 
' * COPYRIGHT AND LICENSE NOTICE
' *
' * The License allows you to download, install and use one or more free copies of this program 
' * for private, public or commercial use.
' * 
' * You may not sell, repackage, redistribute or modify any part of the code or application, 
' * or represent it as being your own work without written permission from the author.
' * You can however modify source code (at your own risk) to adapt it to your specific needs 
' * or to integrate it into your site. 
' *
' * All links and information about the copyright MUST remain unchanged; 
' * you can modify or remove them only if expressly permitted.
' * In particular the license allows you to change the application logo with a personal one, 
' * but it's absolutly denied to remove copyright information,
' * including, but not limited to, footer credits, inline credits metadata and HTML credits comments.
' *
' * For the full copyright and license information, please view the LICENSE.htm
' * file that was distributed with this source code.
' *
' * Removal or modification of this copyright notice will violate the license contract.
' *
' *
' * @category        ASP Stats Generator
' * @package         ASP Stats Generator
' * @author          Simone Carletti <weppos@weppos.net>
' * @copyright       2003-2008 Simone Carletti
' * @license         http://www.weppos.com/asg/en/license.asp
' * @version         SVN: $Id: datetime.asp 123 2008-04-22 20:06:08Z weppos $
' */

'/* 
' * Any disagreement of this license behaves the removal of rights to use this application.
' * Licensor reserve the right to bring legal action in the event of a violation of this Agreement.
' */


'
' Returns a "Datestamp" for given dtm.
' The datestamp is simply a shorten representation for a date time
' in the format %Y%m%d.
'
public function asgDatestamp(dtm)
  asgDatestamp = Clng(Year(dtm) &_
                      Right("0" & Month(dtm), 2) &_ 
                      Right("0" & Day(dtm), 2) )
end function


'
' Computes and return the elaboration time
' between timerStart and timerEnd.
'
' @param  timer   timerStart
' @param  timer   timerEnd
'
public function asgComputeElabtime(timerStart, timerEnd)
  asgComputeElabtime = FormatNumber(timerEnd - timerStart, 4)
end function




%>
