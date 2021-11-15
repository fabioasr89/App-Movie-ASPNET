using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace app_movie
{
    public partial class _Default : Page
    {
        private SqlConnection SqlConnection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DBMovie"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            String Query = "";
            String Query2 = "";
            try
            {
                if (SqlConnection.State == System.Data.ConnectionState.Open)
                {
                    SqlConnection.Close();
                }
                //recuperiamo solo la prima volta, al load della pagina
                //successivamente dovremo caricare il contenuto dinamico
                if (!Page.IsPostBack)
                {
                    SqlConnection.Open();
                    Query = "SELECT film.filmid,film.titolo,film.anno,film.copertina";
                    Query = Query + " " + "FROM dbo.film as film LEFT JOIN dbo.ass_film_genere assfg on (assfg.fk_film=film.filmid)" + " "
                        + "LEFT JOIN dbo.genere as genere on (genere.genereid=assfg.fk_genere)";
                    SqlDataSource1.SelectCommand = Query;
                    SqlDataSource1.ConnectionString = SqlConnection.ConnectionString;
                    GridView1.DataSource = SqlDataSource1;
                    GridView1.DataBind();
                    //popoliamo la select dei generi necessaria per i filtri di ricerca
                    Query2 = "Select gender.genereid as id,gender.descrizione as descrizione from dbo.genere gender";
                    SqlDataSource1.SelectCommand = Query2;
                    Generi.DataSource = SqlDataSource1;
                    Generi.DataValueField = "id";
                    Generi.DataTextField = "descrizione";
                    //Generi.AutoPostBack = true;
                    Generi.DataBind();
                    //inserisco come primo elemento una descrizione di default
                    Generi.Items.Insert(0, new ListItem("Filtra per genere", "0"));


                }


            }
            catch (Exception Exception)
            {
                Console.WriteLine(Exception.Message);
            }

        }
        //intercetto l'associazione del dato alla cella, e modifico il valore della terza, che è la copertina
        //dei film 
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Console.WriteLine("sei qua");
                //recuperiamo le immagine in una cartella di risorse del server Images/idFilm/nome_immagine
                e.Row.Cells[3].Text = Server.HtmlDecode(@"<img src=""./Image/" + e.Row.Cells[3].Text + "\" class=\"img-thumbnail\" style=\"max-width:20%; align:center;\" />");
            }
        }


        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.GridView GridView1 = (System.Web.UI.WebControls.GridView)sender;
            //AGGIUNGO LA SEZIONE <thead> nel controllo, visto che di default non è presente
            //ad essa associo la classe di bootstrap per la formattazione grafica
            if (GridView1.Rows.Count > 0)
            {
                GridView1.CssClass = "table table-hover";
                GridView1.UseAccessibleHeader = true;
                GridView1.HeaderRow.TableSection = System.Web.UI.WebControls.TableRowSection.TableHeader;
                GridView1.HeaderStyle.CssClass = "thead-dark";
                GridView1.HeaderRow.CssClass = "col";



            }


        }



        protected void Ricerca_Click(object sender, EventArgs e)
        {
            Dictionary<String, String> map = GeneraQueryConFiltri();
            String Query = map["query"];

            try
            {
                if (Query != "")
                {
                    GridView1.DataSource = null;
                    SqlDataSource1.ConnectionString = SqlConnection.ConnectionString;
                    SqlDataSource1.SelectCommand = Query;
                    if (map.ContainsKey("genere"))
                    {
                        Parameter Parameter = new Parameter("IdGenere");
                        Parameter.DefaultValue = map["genere"];
                        SqlDataSource1.SelectParameters.Add(Parameter);
                    }
                    if (map.ContainsKey("titolo"))
                    {
                        Parameter Parameter = new Parameter("Titolo");
                        Parameter.DefaultValue = map["titolo"];
                        SqlDataSource1.SelectParameters.Add(Parameter);
                    }

                    GridView1.DataSource = SqlDataSource1;
                    GridView1.DataBind();

                }

            }
            catch (Exception Exc)
            {
                Console.WriteLine(Exc.Message);
            }
        }

        /**Genera la query parametrica recuperando, se esistono, i filtri di ricerca */
        public Dictionary<String, String> GeneraQueryConFiltri()
        {
            Dictionary<String, String> map = new Dictionary<String, String>();
            Int32 IdGenere = 0;
            String TitoloTx = null;
            String Query = "SELECT film.filmid,film.titolo,film.anno,film.copertina";
            Query = Query + " " + "FROM dbo.film as film LEFT JOIN dbo.ass_film_genere assfg on (assfg.fk_film=film.filmid)" + " "
                + "LEFT JOIN dbo.genere as genere on (genere.genereid=assfg.fk_genere) Where 1=1";
            try
            {
                if (Generi.SelectedValue != "0")
                {
                    IdGenere = Int32.Parse(Generi.SelectedValue);
                    if (IdGenere != 0)
                    {
                        Query = Query + " " + "and assfg.fk_genere=@IdGenere";
                        map.Add("genere", Generi.SelectedValue);
                    }
                }
                if (Titolo.Text != null && !"".Equals(Titolo.Text))
                {
                    TitoloTx = Titolo.Text;
                    Query = Query + " " + "and film.titolo LIKE '%'+@Titolo+'%'";
                    map.Add("titolo", Titolo.Text);
                }


                map.Add("query", Query);

            }
            catch (Exception Exc)
            {
                Console.WriteLine(Exc.Message);
            }
            return map;
        }

        protected void Generi_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(Generi.SelectedValue.ToString());
            Generi.Focus();
        }
    
    }
}