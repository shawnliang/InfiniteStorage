package com.waveface.sync.util;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.MalformedURLException;
import java.net.URL;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.List;
import java.util.Locale;
import java.util.StringTokenizer;
import java.util.TimeZone;
import java.util.UUID;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;

import android.content.Context;
import android.net.Uri;

import com.google.gson.stream.JsonReader;
import com.waveface.sync.R;

public class StringUtil {
	/**
	 * SimpleDateFormat pattern for an ISO 8601 date
	 */
	public static String ISO_8601_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";
	public static String DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
	public static String SIMPLE_DATE_FORMAT = "MM/dd/yyyy";
	public static String TIME_FORMAT = "hh:mm a";
	public static String MEDIA_DATE_FORMAT = "yyyyMMdd";
	public static final long ONE_KB = 1024;
	public static final long ONE_MB = ONE_KB * ONE_KB;
	public static final long ONE_GB = ONE_KB * ONE_MB;

	/**
	 * Parse an ISO 8601 date converting ParseExceptions to a null result;
	 */
	public static Date parseDate(String string) {
		DateTimeFormatter parser = ISODateTimeFormat.dateTimeNoMillis();
		return parser.parseDateTime(string).toDate();
	}

	public static Date parse(String input) throws java.text.ParseException {
		// NOTE: SimpleDateFormat uses GMT[-+]hh:mm for the TZ which breaks
		// things a bit. Before we go on we have to repair this.
		SimpleDateFormat df = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ssz");

		// this is zero time so we need to add that TZ indicator for
		if (input.endsWith("Z")) {
			input = input.substring(0, input.length() - 1) + "GMT-00:00";
		} else {
			int inset = 6;

			String s0 = input.substring(0, input.length() - inset);
			String s1 = input.substring(input.length() - inset, input.length());

			input = s0 + "GMT" + s1;
		}

		return df.parse(input);
	}

	public static String changeToLocalString(String input) {
		Date date = null;
		try {
			date = parse(input);
		} catch (ParseException e) {
			e.printStackTrace();
		}
		if (date == null) {
			return null;
		} else {
			long selectedDate = date.getTime();
			long offsetFromUtc = Calendar.getInstance().getTimeZone()
					.getOffset(0);
			date = new Date(selectedDate - offsetFromUtc * 2);
			return formatDate(date);
		}
	}
	public static String changeToLocalString(long time ,String format) {
		SimpleDateFormat sdf = new SimpleDateFormat(format);
		String timeString = sdf.format(new Date(time));
//		String timeString = sdf.format(new Date(time * 1000));
		return changeToLocalString(timeString);
	}

	public static String displayLocalTime(String input, String formatPattern) {
		Date date = null;
		try {
			date = parse(input);
		} catch (ParseException e) {
			e.printStackTrace();
		}
		return new SimpleDateFormat(formatPattern).format(date);
	}

	public static String getWeekDayOfTimestamp(String timestamp, Locale locale) {
		Date date = parseDate(timestamp);
		Calendar cal = new GregorianCalendar(TimeZone.getTimeZone("GMT"));
		cal.setTime(date);
		int DOW = cal.get(Calendar.DAY_OF_WEEK);
		String DOWString = "";
		switch (DOW) {
		case 1:
			if (locale.getLanguage().equals("zh")) {
				DOWString = "星期日";
			} else {
				DOWString = "Sunday";
			}
			break;
		case 2:
			if (locale.getLanguage().equals("zh")) {
				DOWString = "星期一";
			} else {
				DOWString = "Monday";
			}
			break;
		case 3:
			if (locale.getLanguage().equals("zh")) {
				DOWString = "星期二";
			} else {
				DOWString = "Tuesday";
			}
			break;
		case 4:
			if (locale.getLanguage().equals("zh")) {
				DOWString = "星期三";
			} else {
				DOWString = "Wednesday";
			}
			break;
		case 5:
			if (locale.getLanguage().equals("zh")) {
				DOWString = "星期四";
			} else {
				DOWString = "Thursday";
			}
			break;
		case 6:
			if (locale.getLanguage().equals("zh")) {
				DOWString = "星期五";
			} else {
				DOWString = "Friday";
			}
			break;
		case 7:
			if (locale.getLanguage().equals("zh")) {
				DOWString = "星期六";
			} else {
				DOWString = "Saturday";
			}
			break;
		}
		DateFormat longFormat = DateFormat.getDateInstance(DateFormat.LONG,
				locale);
		String elapsedTime = longFormat.format(date);

		if (locale.getLanguage().equals("zh")) {
			int pos = elapsedTime.indexOf("年");
			if (pos != -1) {
				String tempString = elapsedTime.substring("年".length() + pos)
						+ "," + elapsedTime.substring(0, pos);
				elapsedTime = tempString;
			} else {
				longFormat = DateFormat.getDateInstance(DateFormat.LONG,
						Locale.US);
				elapsedTime = longFormat.format(date);
			}
		}
		DOWString += ", " + elapsedTime;
		return DOWString;
	}

	public static String getDeviceView(String from, String codeName,
			Locale locale) {
		String deviceViewString = null;
		if (Locale.getDefault().getLanguage().equals("zh")) {
			deviceViewString = from + codeName;
		} else {
			deviceViewString = from + " " + codeName;
		}
		return deviceViewString;
	}

	public static String incrementSeconds(String timestamp, int seconds) {
		Date date = parseDate(timestamp);
		long longDate = date.getTime();
		longDate += seconds * 1000;
		Date date2 = new Date(longDate);
		Calendar cal = new GregorianCalendar(TimeZone.getTimeZone("GMT"));
		cal.setTime(date2);
		return (cal.get(Calendar.YEAR) + "-"
				+ addZeroPrefix(cal.get(Calendar.MONTH) + 1) + "-"
				+ addZeroPrefix(cal.get(Calendar.DATE)) + "T"
				+ addZeroPrefix(cal.get(Calendar.HOUR_OF_DAY)) + ":"
				+ addZeroPrefix(cal.get(Calendar.MINUTE)) + ":"
				+ addZeroPrefix(cal.get(Calendar.SECOND)) + "Z");
	}

	public static String addZeroPrefix(int value) {
		if (value <= 9) {
			return "0" + value;
		} else {
			return "" + value;
		}
	}

	public static String getTime(String string) {
		DateTimeFormatter parser = ISODateTimeFormat.dateTimeNoMillis();
		return new SimpleDateFormat(TIME_FORMAT).format(parser.parseDateTime(
				string).toDate());
	}

	public static String getTimeWithFromat(String string, String format) {
		DateTimeFormatter parser = ISODateTimeFormat.dateTimeNoMillis();
		return new SimpleDateFormat(format).format(parser.parseDateTime(string)
				.toDate());
	}

	public static String getPostInfo(String timestamp, int attachmentCount,
			String photoString, String from, String device) {
		String elapsedTime = StringUtil.getTime(timestamp);
		StringBuilder builder = new StringBuilder();
		if (Locale.getDefault().getLanguage().equals("zh")) {
			if (attachmentCount > 0) {
				builder.append(attachmentCount);
				builder.append(photoString);
				// builder.append(", ");
			}
			builder.append(" ").append(elapsedTime).append(from).append(device);
		} else {
			if (attachmentCount > 0) {
				builder.append(attachmentCount);
				builder.append(" ");
				builder.append(photoString);
				builder.append(" ");
			}
			builder.append("Posted at ").append(elapsedTime).append(" ")
					.append(from).append(" ").append(device);
		}
		return builder.toString();
	}

	public static String changeDateOutput(String dateString, Locale locale) {
		// Locale locale = new Locale(localeString1, localeString2);
		// Locale locale = Locale.TAIWAN;
		DateFormat longFormat = DateFormat.getDateInstance(DateFormat.LONG,
				locale);
		String elapsedTime = longFormat
				.format(StringUtil.parseDate(dateString));
		if (locale.getLanguage().equals("zh")) {
			int pos = elapsedTime.indexOf("年");
			if (pos != -1) {
				String tempString = elapsedTime.substring("年".length() + pos)
						+ "," + elapsedTime.substring(0, pos);
				elapsedTime = tempString;
			} else {
				longFormat = DateFormat.getDateInstance(DateFormat.LONG,
						Locale.US);
				elapsedTime = longFormat.format(StringUtil
						.parseDate(dateString));
				// String[] monthday = elapsedTime.split(",")[0].split(" ");
				// String year = elapsedTime.split(",")[1];
			}
		}
		return elapsedTime;
	}

	public static String getSimpleDate(String dateString) {
		SimpleDateFormat dateFormatGmt = new SimpleDateFormat(
				SIMPLE_DATE_FORMAT);
		dateFormatGmt.setTimeZone(TimeZone.getTimeZone("GMT"));
		return dateFormatGmt.format(StringUtil.parseDate(dateString)) + "";
	}

	public static String changeLocalDateOutput(String dateString, Locale locale) {
		SimpleDateFormat dateFormatGmt = new SimpleDateFormat(DATE_FORMAT);
		dateFormatGmt.setTimeZone(TimeZone.getTimeZone("GMT"));
		return dateFormatGmt.format(StringUtil.parseDate(dateString)) + "";
	}

	public static String changeLocalDateOutputByTimeZone(String dateString,
			Locale locale) {
		TimeZone tz = TimeZone.getDefault();
		SimpleDateFormat dateFormatGmt = new SimpleDateFormat(DATE_FORMAT,
				locale);
		dateFormatGmt.setTimeZone(TimeZone.getTimeZone(tz.getID()));
		return dateFormatGmt.format(StringUtil.parseDate(dateString)) + "";
	}

	public static String[] getMonthAndDay(String dateString, Locale locale) {
		String[] values = new String[2];
		// Locale locale = new Locale(localeString1, localeString2);
		// Locale locale = Locale.TAIWAN;
		boolean useUSLOCALE = false;
		DateFormat longFormat = DateFormat.getDateInstance(DateFormat.LONG,
				locale);
		String elapsedTime = longFormat
				.format(StringUtil.parseDate(dateString));
		if (locale.getLanguage().equals("zh")) {
			int pos = elapsedTime.indexOf("年");
			if (pos != -1) {
				String tempString = elapsedTime.substring("年".length() + pos);
				elapsedTime = tempString;
				values[0] = tempString.substring(0, tempString.indexOf("月")
						+ "月".length());
				elapsedTime = tempString.substring(tempString.indexOf("月")
						+ "月".length());
				values[1] = elapsedTime.substring(0, elapsedTime.indexOf("日"));
			} else {
				useUSLOCALE = true;
			}
		} else {
			useUSLOCALE = true;
		}
		if (useUSLOCALE) {
			longFormat = DateFormat.getDateInstance(DateFormat.LONG, Locale.US);
			elapsedTime = longFormat.format(StringUtil.parseDate(dateString));
			elapsedTime = elapsedTime.substring(0, elapsedTime.indexOf(","));
			values = elapsedTime.split(" ");
			values[0] = values[0].toUpperCase().substring(0, 3);
		}
		if (values[1].length() == 1) {
			values[1] = "0" + values[1];
		}
		return values;
	}

	/**
	 * Format a date as an ISO 8601 string, return "" for a null date
	 */
	public static String formatDate(Date date) {
		if (date == null) {
			return "";
		}
		return new SimpleDateFormat(ISO_8601_DATE_FORMAT).format(date);
	}

	/**
	 * Format a date as an ISO 8601 string, return "" for a null date
	 */
	public static String getLocalDate() {
		SimpleDateFormat dateFormatGmt = new SimpleDateFormat(
				ISO_8601_DATE_FORMAT);
		dateFormatGmt.setTimeZone(TimeZone.getTimeZone("GMT"));
		return dateFormatGmt.format(new Date()) + "";
	}

	public static boolean before(String ISO8601_DATE1, String ISO8601_DATE2) {
		if (ISO8601_DATE1 == null)
			return true;
		return before(parseDate(ISO8601_DATE1), parseDate(ISO8601_DATE2));
	}

	public static boolean before(Date date1, Date date2) {
		return date1.before(date2);
	}

	public static String replaceAllWords(String original, String find,
			String replacement) {
		StringBuilder result = new StringBuilder(original.length());
		String delimiters = "+-*/(),. ";
		StringTokenizer st = new StringTokenizer(original, delimiters, true);
		while (st.hasMoreTokens()) {
			String w = st.nextToken();
			if (w.equals(find)) {
				result.append(replacement);
			} else {
				result.append(w);
			}
		}
		return result.toString();
	}

	public static String replaceEscape(String ori) {
		// Server should decrese this
		ori = ori.replaceAll("\\\\n", "");
		ori = ori.replace("\"{", "{");
		ori = ori.replace("}\"", "}");
		// unescape
		// StringEscapeUtils.unescapeHtml(ori);
		ori = ori.replaceAll("\\\\", "");
		ori = ori.replaceAll("\\\\/", "/");
		return ori;
	}

	public static String replaceHtmlExcapeString(String ori) {
		ori = ori.replaceAll("\\\\\"", "\\\"");
		// ori = ori.replaceAll("\\<.*?>","");
		// ori = ori.replaceAll("&nbsp;","");
		// ori = ori.replaceAll("&amp;","");
		return ori;
	}

	public static String extractAllURL(String text) {
		StringBuilder builder = new StringBuilder();
		String returnText = null;
		int startPos = 0;
		int endPos = 0;
		int lastPos = 0;
		boolean run = true;
		Log.d("STRINGUTIL", "extractAllURL ORI:" + text);
		boolean startYoutube = false;
		if ((text.startsWith("http://") || text.startsWith("https://"))
				&& text.contains("youtube")) {
			startYoutube = true;
		}
		while (run) {
			// Log.d("STRINGUTIL","extractAllURL:text.substring("+lastPos+"):"+text.substring(lastPos));
			returnText = extractFirstURL(text.substring(lastPos));
			Log.d("STRINGUTIL", "extract URL:" + returnText);
			if (returnText != null) {
				if (!startYoutube) {
					startPos = text.substring(lastPos).indexOf(returnText)
							+ lastPos;
					endPos = startPos + returnText.length();
					lastPos = endPos;
				} else {
					startPos = 0;
					endPos = text.length();
					lastPos = endPos;
				}
				builder.append(startPos + "," + endPos + "@");
				Log.d("STRINGUTIL", "extractAllURL:" + startPos + "," + endPos);
				// Log.d("STRINGUTIL","extractAllURL:"+text.substring(startPos,endPos));
			} else {
				run = false;
			}
		}
		return builder.toString();
	}

	public static String extractFirstURL(String text) {
		if (text == null)
			return null;
		text = text.replaceAll("\n", " ");
		text = text.replaceAll("\r", " ");
		String returnURL = null;
		String upperText = text.toUpperCase();
		String HttpHeader = "HTTP://";
		String HttpsHeader = "HTTPS://";
		String WWWHeader = "WWW.";
		String ContainHttpHeader = " HTTP://";
		String ContainHttpsHeader = " HTTPS://";
		String ContainWWWHeader = " WWW.";

		Log.d("STRINGUTIL", "HTTP[ORI_URL]:" + text);
		String evernoteStartString = "HTTPS://WWW.EVERNOTE.COM/SHARD/";
		String twitterEndString = "--HTTP://TWITTER.COM/";
		String youtubePatternString = "HTTP://M.YOUTUBE.COM/";
		boolean isEvernotePattern = upperText.startsWith(evernoteStartString);
		if (isEvernotePattern) {
			String newline = System.getProperty("line.separator");
			int position = upperText.indexOf(newline);
			if (position > 0) {
				text = text.substring(0, position);
			}
			returnURL = text;
		} else {
			boolean isTwitterPattern = upperText.contains(twitterEndString);
			boolean isYoutubePattern = upperText
					.startsWith(youtubePatternString);
			if (isTwitterPattern) {
				int position = upperText.indexOf(twitterEndString);
				upperText = upperText.substring(0, position);
				text = text.substring(0, position);
				position = upperText.indexOf("\"", 0);
				upperText = upperText.substring(position + 1);
				text = text.substring(position + 1);
			} else if (isYoutubePattern) {
				String tempString = "";
				int indexPosition = upperText.indexOf("/INDEX");
				if (indexPosition < 0) {
					indexPosition = upperText.indexOf("/#");
				}
				if (indexPosition > 0) {
					int watchPosition = upperText.indexOf("/WATCH?");
					if (watchPosition > 0) {
						tempString = upperText.substring(0, indexPosition);
						tempString += upperText.substring(watchPosition);
						upperText = tempString;
						tempString = text.substring(0, indexPosition);
						tempString += text.substring(watchPosition);
						text = tempString;
					}
				}
			}
			boolean startedHTTP = upperText.startsWith(HttpHeader);
			boolean startedHTTPS = upperText.startsWith(HttpsHeader);
			boolean startedWWW = upperText.startsWith(WWWHeader);

			if (!startedHTTP && !startedHTTPS && !startedWWW) {
				boolean hasHTTP = upperText.contains(ContainHttpHeader);
				int httpStartPosition = 0;
				int httpEndPosition = 0;
				if (hasHTTP) {
					httpStartPosition = upperText.indexOf(ContainHttpHeader, 0);
					if (isTwitterPattern) {
						httpEndPosition = upperText.indexOf("\"",
								upperText.length() - 2);
						if (httpEndPosition > 0) {
							hasHTTP = true;
							Log.d("STRINGUTIL",
									"HTTP[URL]:"
											+ text.substring(httpStartPosition,
													httpEndPosition));
						} else {
							hasHTTP = false;
						}
					} else {
						httpEndPosition = upperText.indexOf(" ",
								httpStartPosition + 1);
						if (httpEndPosition > 0) {
							hasHTTP = true;
							Log.d("STRINGUTIL",
									"HTTP[URL]:"
											+ text.substring(httpStartPosition,
													httpEndPosition));
						} else {
							int totalLength = upperText.length();
							if (totalLength - httpStartPosition > ContainHttpHeader
									.length()) {
								hasHTTP = true;
								httpEndPosition = totalLength;
								Log.d("STRINGUTIL",
										"HTTP[URL]:"
												+ text.substring(
														httpStartPosition,
														httpEndPosition));
							} else {
								hasHTTP = false;
							}
						}
					}

				}
				boolean hasHTTPS = upperText.contains(ContainHttpsHeader);
				int httpsStartPosition = 0;
				int httpsEndPosition = 0;
				if (hasHTTPS) {
					httpsStartPosition = upperText.indexOf(ContainHttpsHeader,
							0);

					httpsEndPosition = upperText.indexOf(" ",
							httpsStartPosition + 1);
					if (httpsEndPosition > 0) {
						hasHTTPS = true;
						Log.d("STRINGUTIL",
								"HTTPS[URL]:"
										+ text.substring(httpsStartPosition,
												httpsEndPosition));
					} else {
						int totalLength = upperText.length();
						if (totalLength - httpsStartPosition > ContainHttpsHeader
								.length()) {
							hasHTTPS = true;
							httpsEndPosition = totalLength;
							Log.d("STRINGUTIL",
									"HTTPS[URL]:"
											+ text.substring(
													httpsStartPosition,
													httpsEndPosition));
						} else {
							hasHTTPS = false;
						}
					}
				}
				boolean hasWWW = upperText.contains(ContainWWWHeader);
				int wwwStartPosition = 0;
				int wwwEndPosition = 0;
				if (hasWWW) {
					wwwStartPosition = upperText.indexOf(ContainWWWHeader, 0);
					wwwEndPosition = upperText.indexOf(" ",
							wwwStartPosition + 1);
					if (wwwEndPosition > 0) {
						hasWWW = true;
						Log.d("STRINGUTIL",
								"WWW[URL]:"
										+ text.substring(wwwStartPosition,
												wwwEndPosition));
					} else {
						int totalLength = upperText.length();
						if (totalLength - wwwStartPosition > ContainWWWHeader
								.length()) {
							hasWWW = true;
							wwwEndPosition = totalLength;
							Log.d("STRINGUTIL",
									"WWW[URL]:"
											+ text.substring(wwwStartPosition,
													wwwEndPosition));
						} else {
							hasWWW = false;
						}
						String tempText = text.substring(wwwStartPosition,
								wwwEndPosition);
						int firstPos = tempText.indexOf(".");
						int secondPos = tempText.indexOf(".", firstPos + 1);
						if (secondPos > -1 && secondPos > firstPos) {
							hasWWW = true;
							Log.d("STRINGUTIL",
									"WWW[URL]:"
											+ text.substring(wwwStartPosition,
													wwwEndPosition));
						} else {
							hasWWW = false;
						}
					}
				}
				if (hasHTTP) {
					if (hasHTTPS) {
						if (hasWWW) {
							if (httpsStartPosition < wwwStartPosition) {
								if (httpStartPosition < httpsStartPosition) {
									returnURL = text.substring(
											httpStartPosition + 1,
											httpEndPosition);
								} else {
									returnURL = text.substring(
											httpsStartPosition + 1,
											httpsEndPosition);
								}
							} else {
								returnURL = text.substring(
										wwwStartPosition + 1, wwwEndPosition);
							}
						} else {
							if (httpStartPosition < httpsStartPosition) {
								returnURL = text.substring(
										httpStartPosition + 1, httpEndPosition);
							} else {
								returnURL = text.substring(
										httpsStartPosition + 1,
										httpsEndPosition);
							}
						}
					} else if (hasWWW) {
						if (httpStartPosition < wwwStartPosition) {
							returnURL = text.substring(httpStartPosition + 1,
									httpEndPosition);
						} else {
							returnURL = text.substring(wwwStartPosition + 1,
									wwwEndPosition);
						}
					} else {
						returnURL = text.substring(httpStartPosition + 1,
								httpEndPosition);
					}
				} else {
					if (hasHTTPS) {
						if (hasWWW) {
							if (httpsStartPosition < wwwStartPosition) {
								returnURL = text.substring(
										httpsStartPosition + 1,
										httpsEndPosition);
							} else {
								returnURL = text.substring(
										wwwStartPosition + 1, wwwEndPosition);
							}
						} else {
							returnURL = text.substring(httpsStartPosition + 1,
									httpsEndPosition);
						}
					} else if (hasWWW) {
						returnURL = text.substring(wwwStartPosition + 1,
								wwwEndPosition);
					}
				}
			} else {
				if (text.contains(" ")) {
					int position = text.indexOf(" ");
					if (startedWWW) {
						String tempText = text.substring(0, position);
						int firstPos = tempText.indexOf(".");
						int secondPos = tempText.indexOf(".", firstPos + 1);
						if (secondPos > -1 && secondPos > firstPos) {
							returnURL = tempText;
						} else {
							returnURL = extractFirstURL(text
									.substring(position));
						}
					} else {
						returnURL = text.substring(0, position);
					}
				} else {
					returnURL = text;
				}
			}
		}
		return returnURL;
	}

	public static String extractURL(String text) {
		String returnURL = null;
		String upperText = text.toUpperCase();
		boolean hasHTTPS = upperText.contains(" HTTPS://");
		int httpsStartPosition = 0;
		int httpsEndPosition = 0;
		boolean hasHTTP = upperText.contains(" HTTP://");
		int httpStartPosition = 0;
		int httpEndPosition = 0;

		boolean startedHTTP = upperText.startsWith("HTTP://");
		boolean startedHTTPS = upperText.startsWith("HTTPS://");

		if (!startedHTTP && !startedHTTPS) {
			if (hasHTTP) {
				httpStartPosition = upperText.indexOf(" HTTP://", 0);
				httpEndPosition = upperText.indexOf(" ", httpStartPosition + 1);

				if (httpEndPosition < 0) {
					hasHTTP = true;
					// log.debug("STRINGUTIL, HTTP[URL]:"+text.substring(httpStartPosition));
				} else if (httpEndPosition >= 0) {
					hasHTTP = true;
					// log.debug("STRINGUTIL, HTTP[URL]:"+text.substring(httpStartPosition,
					// httpEndPosition));
				}

			}

			if (hasHTTPS) {
				httpsStartPosition = upperText.indexOf(" HTTPS://", 0);
				httpsEndPosition = upperText.indexOf(" ",
						httpsStartPosition + 1);
				if (httpsEndPosition < 0) {
					hasHTTPS = true;
					// Log.d("STRINGUTIL, HTTP[URL]:"+text.substring(httpsStartPosition));
				} else if (httpsEndPosition >= 0) {
					hasHTTPS = true;
					// log.debug("STRINGUTIL, HTTPS[URL]:"+text.substring(httpsStartPosition,
					// httpsEndPosition));
				}
			}
			if (hasHTTP) {
				if (hasHTTPS) {
					if (httpStartPosition < httpsStartPosition) {
						returnURL = text.substring(httpStartPosition + 1,
								httpEndPosition);
					} else {
						returnURL = text.substring(httpsStartPosition + 1,
								httpsEndPosition);
					}
				} else {
					if (httpEndPosition < 0) {
						returnURL = text.substring(httpStartPosition + 1);
					} else {
						returnURL = text.substring(httpStartPosition + 1,
								httpEndPosition);
					}
				}
			} else {
				if (hasHTTPS) {
					if (httpsEndPosition < 0) {
						returnURL = text.substring(httpsStartPosition + 1);
					} else {
						returnURL = text.substring(httpsStartPosition + 1,
								httpsEndPosition);
					}
				}
			}
		}
		// start with http://
		else {
			String empty = " ";
			int position = text.indexOf(empty);
			if (position > -1) {
				text = text.substring(0, position);
			}
			returnURL = text;
		}
		return returnURL;
	}

	public static String addHttpString(String text) {
		String url = "";
		String temp = "";
		url = extractURL(text);
		if (url != null) {
			int position = text.indexOf(url);
			temp = text.substring(0, position);
			temp += String.format("<u>%s</u>",
					"<font color=\"#f8f8f8\" size=\"12sp\"><a href=\"" + url
							+ "\">" + url + "</a></font>");
			temp += addHttpString(text.substring(position + url.length()));
		} else {
			temp = text;
		}
		return temp;
	}

	public static String generateTag(String mainTag, String subTag) {
		return mainTag + '_' + subTag;
	}

	public static String getMainTag(String tag) {
		String mainTag = null;
		if (tag != null) {
			int divider = tag.indexOf('_');
			if (divider >= 0) {
				mainTag = tag.substring(0, divider);
			}
		}
		return mainTag;
	}

	public static String getSubTag(String tag) {
		String subTag = null;
		if (tag != null) {
			int divider = tag.indexOf('_');
			if (divider >= 0) {
				subTag = tag.substring(divider + 1, tag.length());
			}
		}
		return subTag;
	}

	public static String getUUID() {
		return UUID.randomUUID().toString();
	}

	public static String[] getUUIDS(int count) {
		String[] uuids = new String[count];
		for (int i = 0; i < count; i++) {
			uuids[i] = UUID.randomUUID().toString();
		}
		return uuids;
	}

	public static String generateImageName() {
		Calendar cal = Calendar.getInstance();
		return "IMG_" + new SimpleDateFormat("yyyyMMdd").format(cal.getTime())
				+ "_" + new SimpleDateFormat("HHmmss").format(cal.getTime());
	}

	public static String getUriHost(Uri uri) {
		return uri.getScheme() + "://" + uri.getEncodedAuthority();
	}

	public static String getUriPath(Uri uri) {
		String path = uri.getPath();
		if (uri.getQuery() != null) {
			path += "?" + uri.getQuery();
		}
		return path;
	}

	public static String ToDBC(String input) {
		char[] c = input.toCharArray();
		for (int i = 0; i < c.length; i++) {
			if (c[i] == 12288) {
				c[i] = (char) 32;
				continue;
			}
			if (c[i] > 65280 && c[i] < 65375)
				c[i] = (char) (c[i] - 65248);
		}
		return new String(c);
	}

	public static String getPath(String filename) {
		// return
		// "file:/"+filename.substring(0,filename.lastIndexOf(File.separator)+1);
		String[] arrays = filename.split("/");
		String rtnString = "file:";
		for (int i = 0; i < arrays.length; i++) {
			if (!arrays[i].equals("")) {
				rtnString += "\\" + arrays[i];
			}
		}
		return rtnString;
	}

	public static String getFilename(String fullFilename) {
		String rtnString = fullFilename;
		fullFilename = fullFilename.replaceAll("\\\\", "/");
		int pos = fullFilename.lastIndexOf(File.separator);
		if (pos != -1) {
			rtnString = fullFilename.substring(pos + 1);
		}
		return rtnString;
	}

	public static String getEndDate(String strQueryDate) {
		String[] result = { "" };
		String startDateTime = strQueryDate + "000000000";

		try {
			java.util.Calendar calendar = java.util.Calendar.getInstance();
			calendar.set(java.util.Calendar.YEAR,
					Integer.parseInt(startDateTime.substring(0, 4)));
			calendar.set(java.util.Calendar.MONTH,
					Integer.parseInt(startDateTime.substring(4, 6)) - 1);
			calendar.set(java.util.Calendar.DATE,
					Integer.parseInt(startDateTime.substring(6, 8)));
			calendar.set(java.util.Calendar.HOUR_OF_DAY, Integer.parseInt(
					startDateTime.substring(8, 10)));
			calendar.set(java.util.Calendar.MINUTE,
					Integer.parseInt(startDateTime.substring(10, 12)));
			calendar.set(java.util.Calendar.SECOND,
					Integer.parseInt(startDateTime.substring(12, 14)));
			startDateTime = String.valueOf(calendar.getTimeInMillis())
					.substring(0, 10);

		} catch (Exception e) {
			startDateTime = "";

		} finally {
			result[0] = startDateTime;

		}
		return startDateTime;

	}

	public static String getConverDate(Long date) {
		SimpleDateFormat sdf = new SimpleDateFormat(MEDIA_DATE_FORMAT);
		// DateFormat df = DateFormat.getDateInstance();
		return sdf.format(new Date(date * 1000));
	}

	public static String getConverDate(Long date, String format) {
		SimpleDateFormat sdf = new SimpleDateFormat(format);
		// DateFormat df = DateFormat.getDateInstance();
		return sdf.format(new Date(date * 1000));
	}

	public static String generateString(String token, String delimeter,
			int count) {
		StringBuilder buf = new StringBuilder();
		for (int i = 0; i < count; i++) {
			buf.append(token);
			if (i != (count - 1)) {
				buf.append(delimeter);
			}
		}
		return buf.toString();
	}

	public static String pad(int c) {
		if (c >= 10)
			return String.valueOf(c);
		else
			return "0" + String.valueOf(c);
	}

	public void readJsonStream(InputStream in) throws IOException {
		JsonReader reader = new JsonReader(new InputStreamReader(in, "UTF-8"));
		// List<Message> messages = new ArrayList<Message>();
		reader.beginArray();
		// while (reader.hasNext()) {
		// Message message = gson.fromJson(reader, Message.class);
		// messages.add(message);
		// }
		reader.endArray();
		reader.close();
	}
	public static int getTimezoneHour() {
		return getTimezoneMinute()/60;
	}
	public static int getTimezoneMinute() {
		Calendar cal = Calendar.getInstance();
		cal.setTimeInMillis(System.currentTimeMillis());
		int dayLightSaving = cal.get(Calendar.DST_OFFSET);
		cal.setTimeInMillis(cal.getTimeInMillis() - dayLightSaving);
		return cal.getTimeZone().getRawOffset() / 1000 / 60;
	}

	// GET DAY BRFORE DAY PLUS DAY
	public static String dateForward(String day, int addDays) {
		Calendar calendar = getCalendar(day);
		calendar.add(Calendar.DATE, addDays);
		StringBuffer DateString = new StringBuffer();
		DateString.append(calendar.get(Calendar.YEAR)).append("-");
		int Month = calendar.get(Calendar.MONTH) + 1;
		int Day = calendar.get(Calendar.DATE);
		if (Month < 10) {
			DateString.append("0");
		}
		DateString.append(Month).append("-");
		if (Day < 10) {
			DateString.append("0");
		}
		DateString.append(Day);
		return DateString.toString();
	}

	public static String dateReverse(String day, int delDays) {
		return dateForward(day, (0 - delDays));
	}

	public static String[] getDateStringReverse(String startDay, int days){
		String[] dayDatas = new String[days];
		dayDatas[0] = startDay;
		for(int i = 0 ; i < days; i++){
			if(i>0){
				dayDatas[i] = dateForward(dayDatas[i-1], -1);
			}
		}
		return dayDatas;
	}

	//FORMAT YYYY/MM/DD
	private static Calendar getCalendar(String day) {
		GregorianCalendar calendar = (GregorianCalendar) Calendar.getInstance();
		int Year = Integer.parseInt(day.substring(0, 4));
		int Month = Integer.parseInt(day.substring(5, 7)) - 1;
		int Day = Integer.parseInt(day.substring(8, 10));
		calendar.set(Year, Month, Day);
		return calendar;
	}

	public static String byteCountToDisplaySize(long size) {
		String displaySize;

		if (size / ONE_GB > 0) {
			displaySize = String.valueOf(size / ONE_GB) + " GB";
		} else if (size / ONE_MB > 0) {
			displaySize = String.valueOf(size / ONE_MB) + " MB";
		} else if (size / ONE_KB > 0) {
			displaySize = String.valueOf(size / ONE_KB) + " KB";
		} else {
			displaySize = String.valueOf(size) + " bytes";
		}
		return displaySize;
	}
	public static String[] byteCountToSizeAndUnit(long size) {
		String displaySize = null;
		String unit = null;
		if (size / ONE_GB > 0) {
			displaySize = String.valueOf(size / ONE_GB);
			unit = "GB";
		} else if (size / ONE_MB > 0) {
			displaySize = String.valueOf(size / ONE_MB);
			unit = "MB";
		} else if (size / ONE_KB > 0) {
			displaySize = String.valueOf(size / ONE_KB);
			unit = "KB";
		} else {
			displaySize = String.valueOf(size);
			unit = "bytes";
		}
		return new String[]{displaySize,unit};
	}


	public static List<Date> getBetweenDayList(String sDate,String eDate) throws ParseException{

		List<Date> dates = new ArrayList<Date>();
		DateFormat formatter ;
		formatter = new SimpleDateFormat("yyyy-MM-dd");
		Date  startDate = formatter.parse(sDate);
		Date  endDate = formatter.parse(eDate);
		long interval = 24*1000 * 60 * 60; // 1 hour in millis
		long endTime = endDate.getTime() ; // create your endtime here, possibly using Calendar or Date
		long curTime = startDate.getTime();
		while (curTime <= endTime) {
		    dates.add(new Date(curTime));
		    curTime += interval;
		}
		// sort by date  desc
		Collections.sort(dates,
        	        new Comparator<Date>() {
        	            @Override
						public int compare(Date o2, Date o1) {
        	                return o1.compareTo(o2);
        	            }
        	        });
		return dates;
	}


	public static ArrayList<Date> getDayList(int addMon) {

		String str_date = "";
		String end_date = "";
		ArrayList<Date> dayList = null;
		Calendar calendar = Calendar.getInstance();

		SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd",
				Locale.US);
		end_date = formatter.format(calendar.getTime());
		calendar.add(Calendar.MONTH, addMon); // pre month
		str_date = formatter.format(calendar.getTime());

		try {
			dayList = (ArrayList<Date>) StringUtil.getBetweenDayList(str_date,
					end_date);

		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return dayList;

	}
	public static Date getDay(Calendar startDate, int diffDay) {
		Calendar dayTarget = Calendar.getInstance();
		dayTarget.set(
				startDate.get(Calendar.YEAR),
				startDate.get(Calendar.MONTH),
				startDate.get(Calendar.DAY_OF_MONTH)-diffDay);
		String day = dayTarget.get(Calendar.YEAR) + "/"
				+ (dayTarget.get(Calendar.MONTH)+1) + "/"
				+ dayTarget.get(Calendar.DAY_OF_MONTH);
		Date target = null;
		try {
			target = new SimpleDateFormat("yyyy/MM/dd", Locale.getDefault()).parse(day);
		} catch (ParseException e) {
			e.printStackTrace();
		}
		return target;
	}

	public static ArrayList<Date> getDayStringsBackward(Calendar startDate,
			Calendar endDate) {
		ArrayList<Date> list = new ArrayList<Date>();
		while (endDate.compareTo(startDate) <= 0) {
			String curDate = endDate.get(Calendar.YEAR) + "/"
					+ (endDate.get(Calendar.MONTH)+1) + "/"
					+ endDate.get(Calendar.DAY_OF_MONTH);
			try {
				list.add(0, new SimpleDateFormat("yyyy/MM/dd", Locale.getDefault()).parse(curDate));
			} catch (ParseException e) {
				e.printStackTrace();
			}

			endDate.add(Calendar.DAY_OF_MONTH, 1);
		}
		return list;
	}

	public static ArrayList<Date> getDayStringsBackwardReverseOrder(Calendar startDate,
			Calendar endDate) {
		ArrayList<Date> list = new ArrayList<Date>();
		while (endDate.compareTo(startDate) <= 0) {
			String curDate = endDate.get(Calendar.YEAR) + "/"
					+ (endDate.get(Calendar.MONTH)+1) + "/"
					+ endDate.get(Calendar.DAY_OF_MONTH);
			try {
				list.add(new SimpleDateFormat("yyyy/MM/dd", Locale.getDefault()).parse(curDate));
			} catch (ParseException e) {
				e.printStackTrace();
			}

			endDate.add(Calendar.DAY_OF_MONTH, 1);
		}
		return list;
	}

	public static String getDomainName(String urlAddress){

		 URL url=null;
		 String domain;
		try {
			url = new URL(urlAddress);
		} catch (MalformedURLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return  domain = url.getHost();
	}

}
