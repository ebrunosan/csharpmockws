using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Runtime.Caching;

namespace WS2Play.Controllers {
	public class ValuesController : ApiController {
		public class Notes {
			public Notes(int _Id, string _Text) {
				this.Id = _Id;
				this.Text = _Text;
			}
			public int Id { get; set; }
			public string Text { get; set; }
		}

		private List<Notes> _notes = new List<Notes>();
		private int _lastId = 0;

		public ValuesController() {
			ObjectCache cache = MemoryCache.Default;

			var	notesMemory = cache.Get("NotesMemory") as List<Notes>;

			if (notesMemory != null) {
				_notes = notesMemory;
			} else {
				CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) };
				cache.Add("NotesMemory", _notes, policy);
			}

			// return last inserted Id if there is some, zero otherwise
			_lastId = _notes.Count == 0 ? 0 : _notes[_notes.Count - 1].Id;
		}

		// GET api/values
		public IEnumerable<Notes> Get() {
			return _notes;
		}

		// GET api/values/5
		public Notes Get(int id) {
			if (id < 0) {
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}

			var idx = _notes.FindIndex(note => note.Id == id);
			if (idx == -1) {
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			return _notes[idx];
		}

		// POST api/values
		public Notes Post([FromBody]Notes notes) {
			_lastId++;		// Increment lastId

			var newNote = new Notes(_lastId, notes.Text);
			_notes.Add(newNote);

			return newNote;
		}

		// DELETE api/values/5
		public IHttpActionResult Delete(int id) {
			if (id < 0) {
				return BadRequest("Invalid note's key");
			}

			var idx = _notes.FindIndex(note => note.Id == id);
			if (idx == -1) {
				return NotFound();
			}

			_notes.RemoveAt(idx);
			return Ok();
		}
	}
}
