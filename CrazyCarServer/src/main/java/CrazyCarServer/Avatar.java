package CrazyCarServer;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.List;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import Util.Util;
import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;

/**
 * Servlet implementation class Avatar
 */
public class Avatar extends HttpServlet {
	private static final long serialVersionUID = 1L;
       
    /**
     * @see HttpServlet#HttpServlet()
     */
    public Avatar() {
        super();
         
    }

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		response.setContentType("text/html;charset=UTF-8");
		System.out.println("Avatar ...");
	    int uid = 0;
		String token = request.getHeader("Authorization");
		if  (token == null || !Util.JWT.isLegalJWT(token)){
			System.out.println("illegal JWT");
			return;
		} else{
			uid = Util.JWT.getJWTId(token);
		}			
	
		PrintWriter out = response.getWriter();			
        JSONObject jsonObject = new JSONObject();
		jsonObject.put("code", 200);
		
        JSONObject jbData = new JSONObject();                
		JSONArray jsonArray = new JSONArray();
		List<Integer> allAid = getAllAvatarID();
		for (int i = 0; i < allAid.size(); i++){
			JSONObject jbItem = new JSONObject();
			jbItem.put("is_has", isHasAvatar(allAid.get(i), uid));
			jbItem.put("aid", allAid.get(i));
			jbItem.put("name", getAvatarName(allAid.get(i)));
			jbItem.put("star", getStar(allAid.get(i)));
			jsonArray.add(jbItem);
		}		
		jbData.put("avatars", jsonArray);
		jsonObject.put("data", jbData);
		
		out.println(jsonObject.toString());	
		out.flush();
		out.close();
	}		
	
	private int getStar(int aid) {
		String sql = "select star from avatar_name where aid = "
				+  aid + ";";
		return Util.JDBC.executeSelectInt(sql, "star");
	}
	
	private List<Integer> getAllAvatarID(){
		String sql = "select aid from avatar_name;";
		return Util.JDBC.executeSelectAllInt(sql, "aid");
	}
	
	private boolean isHasAvatar(int aid, int uid){
		String sql = "select aid from avatar_uid where aid = "
				+  aid + " and " + " uid = " + uid + ";";
		return Util.JDBC.executeSelectInt(sql, "aid") != -1;
	}
	
	private String getAvatarName(int aid){
		String sql = "select avatar_name from avatar_name where aid = " + aid + ";";
		return Util.JDBC.executeSelectString(sql, "avatar_name");
	}

	/**
	 * @see HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {	 
		doGet(request, response);
	}

}
